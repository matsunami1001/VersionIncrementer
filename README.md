# VersionIncrementer
C#とVisualBasic用のビルドバージョン更新アプリ
### 使い方
ビルド前イベントに以下のように記述(C# プロジェクトの場合):  
`VersionIncrementer.exe --ProjectName "$(ProjectName)" --AssemblyInfo "$(ProjectPath)Properties\AssemblyInfo.cs"`  
GUIによって、現在のバージョンのそれぞれのセクションをどう更新するかどうかを指定できます。  
##### None
何もしません。
##### Increment
引数に指定された値だけ、数値を増加させます。引数を省略した場合、1だけ増加させます。
##### SetNumber
引数に指定した値を、そのまま設定します。
##### SetDate
現在の時間を、引数に指定した書式で数値にし、その値を設定します。  
[.Netの書式](https://dobon.net/vb/dotnet/string/datetimeformat.html)がそのまま指定できます。  
  
また、引数に`--NoDialog`を指定することで、GUIを表示せずにバージョンを更新します。  
その場合、最後に指定した入力または既定のルール(リビジョンのみ`Increment`)でバージョンを更新します。  
  
キャンセルまたは閉じるボタンでフォームを閉じると、終了コード`1`でプログラムを終了します。
