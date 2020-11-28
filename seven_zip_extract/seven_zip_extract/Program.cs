using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace seven_zip_extract
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            // コマンドライン引数
            if (args.Length != 2)
            {
                Console.WriteLine("コマンドライン引数に、ファイル名を指定して下さい。");
                return 1;
            }

            // 解凍対象のファイル（フルパス）
            var targetFile = args[1];
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
                _ => targetFile,
            };
            File.Move(targetFile, changeFile);

            // TODO debug
            Dbg(settings);
            return 0;
        }
        
        /// <summary>
        /// デバッグ用にコンソール出力します。
        /// </summary>
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