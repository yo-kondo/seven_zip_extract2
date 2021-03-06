﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace SevenZipExtract
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            try
            {
                Execute(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// メイン処理
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        internal static void Execute(IReadOnlyList<string> args)
        {
            // コマンドライン引数
            if (args.Count != 1)
            {
                throw new Exception("コマンドライン引数に、ファイル名を指定して下さい。");
            }

            // 解凍対象のファイル（フルパス）
            var targetFile = args[0];
            if (!File.Exists(targetFile))
            {
                throw new Exception("解凍対象のファイルが存在しません。");
            }

            // 設定ファイル
            var file = File.ReadAllText("appsettings.json");
            var settings = JsonConvert.DeserializeObject<Settings>(file);

            if (!File.Exists(settings.SevenZipPath))
            {
                throw new Exception("7zipのexeが存在しません。");
            }

            // 拡張子を変更したファイル名
            var changeFile = Path.GetExtension(targetFile) switch
            {
                ".ex_" => targetFile.Replace(".ex_", ".exe"),
                ".zi_" => targetFile.Replace(".zi_", ".zip"),
                _ => targetFile
            };
            File.Move(targetFile, changeFile);

            // 7zipで解凍する
            // 7zipの引数: http://fla-moo.blogspot.com/2013/05/7-zip.html
            foreach (var password in settings.ExtractPasswords)
            {
                var proInfo = new ProcessStartInfo
                {
                    FileName = settings.SevenZipPath,
                    ArgumentList =
                    {
                        "x", // x: 解凍
                        "-y", // -y: 強制的に処理を続行
                        $"-p{password}", // -p: パスワードを設定
                        $"-o{Path.GetDirectoryName(changeFile)}", // -o: 出力先を指定
                        $"{changeFile}" // 解凍対象のファイル
                    },
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var process = Process.Start(proInfo);
                if (process == null)
                {
                    throw new Exception("7zipのexe実行に失敗しました。");
                }

                // throw new Exception(process.StandardOutput.ReadToEnd());

                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    Console.WriteLine("解凍に成功しました。");
                    return;
                }
            }

            throw new Exception("解凍に失敗しました。パスワードが間違っている可能性があります。");
        }

        /// <summary>
        /// デバッグ用にコンソール出力します。
        /// </summary>
        /// <param name="obj">出力対象のオブジェクト</param>
        private static void Dbg(object obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }

    /// <summary>
    /// appsettings.json
    /// </summary>
    internal class Settings
    {
        /// <summary>
        /// 7-zipのインストールパス（7z.exeのパス）
        /// </summary>
        [JsonProperty("sevenZipPath")]
        public string SevenZipPath { get; set; } = "";

        /// <summary>
        /// 解凍パスワード（複数可能）
        /// </summary>
        [JsonProperty("extractPasswords")]
        public List<string> ExtractPasswords { get; set; } = new();
    }
}