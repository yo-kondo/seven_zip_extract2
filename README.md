# seven_zip_extract2

7zipで解凍します。（C#版）

## 使い方

`7zipで解凍する.bat`に解凍したいファイルをドラッグ＆ドロップします。  
拡張子が`.ex_`、`.zi_`も自動で変換して解凍します。

## 前提条件

実行するPCに`7-Zip`がインストールされていること。

## 設定

``` json
{
    "sevenZipPath": "C:\\Program Files\\7-Zip\\7z.exe",
    "extractPasswords":
    [
        "password1",
        "password2"
    ]
}
```

* sevenZipPath  
  7-zipのインストールパス（7z.exeのパス）を指定して下さい。
* extractPasswords  
  解凍パスワードを指定して下さい。複数のパスワードを指定することが可能です。

設定ファイルは実行ファイルと同じディレクトリに`appsettings.json`というファイル名で配置して下さい。

## バージョン

* 7-Zip 19.00 (x64)
