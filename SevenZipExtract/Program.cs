using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace SevenZipExtract
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            // コマンドライン引数
            if (args.Length != 1)
            {
                Console.WriteLine("コマンドライン引数に、ファイル名を指定して下さい。");
                return 1;
            }

            // 解凍対象のファイル（フルパス）
            var targetFile = args[0];
            if (!File.Exists(targetFile))
            {
                Console.WriteLine("解凍対象のファイルが存在しません。");
                return 2;
            }

            // 設定ファイル
            var file = File.ReadAllText("appsettings.json");
            var settings = JsonConvert.DeserializeObject<Settings>(file);

            if (!File.Exists(settings.SevenZipPath))
            {
                Console.WriteLine("7zipのexeが存在しません。");
                return 3;
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
                    Console.WriteLine("7zipのexe実行に失敗しました。");
                    return 4;
                }

                // Console.WriteLine(process.StandardOutput.ReadToEnd());

                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    Console.WriteLine("解凍に成功しました。");
                    return 0;
                }
            }

            Console.WriteLine("解凍に失敗しました。パスワードが間違っている可能性があります。");
            return 0;
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
        public List<string> ExtractPasswords { get; set; } = new List<string>();
    }
}