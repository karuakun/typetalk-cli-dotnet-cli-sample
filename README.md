# 動作イメージの取得
## 

## 各自でビルドする場合の手順（セルフコンテナとしてビルドする場合）
### Windows 環境でのビルド
```
git clone https://github.com/karuakun/typetalk-cli-dotnet-cli-sample.git
cd src/GetTypetalkState
dotnet restore
dotnet publish -o ~/out --self-contained -r win-x86
```

### Mac 環境でのビルド
```
git clone https://github.com/karuakun/typetalk-cli-dotnet-cli-sample.git
cd src/GetTypetalkState
dotnet restore
dotnet publish -o ~/out --self-contained -r osx-x64
```

# 初期化
## アプリケーションの登録
TypeTalkのデベロッパーページでアプリケーションを登録し、Client ID と Client Secret を確認する
https://typetalk.com/my/develop/applications

設定例
| 項目 | 設定値 |
|------|--------|
| アプリケーション名 | typetalkcli |
| Grant Type | Client Credentials |
| Description | |


## 設定情報の初期化
config 以外のサブコマンドは初期化が済んでいないと利用できません。

```
.\typetalkcli config --clientId {Client ID} --clientSecret {Client Secret}
```

# スペースの確認

```
> .\typetalkcli getspace -l table
|Key|Name|
|---|----|
|typetalk-spacekey|タイプトークのスペース名|
```


# 参照可能なトピックの一覧

```
.\typetalkcli gettopic {typetalk-spacekey} -l table
|Id|Name|Description|
|--|----|-----------|
|{topickId}|トピック名|概要|
|{topickId}|トピック名|概要|
```

# ポストの取得

```
dotnet runtypetalkcli getpost {typetalk-spacekey} -t {topickId} -from 2018-09-01 -to 2018-09-30 -l table
```

# 特定トピック内で`いいね`された人と`いいね`の数

```
dotnet run likedsummary {typetalk-spacekey} -t {topickId} -from 2018-09-01 -to 2018-09-30 -l table
```

# 特定トピック内で`いいね`した人と`いいね`の数

```
dotnet run likesummary {typetalk-spacekey} -t {topickId} -from 2018-09-01 -to 2018-09-30 -l table
```

# 期間内でいいねの数が多かった投稿
```
dotnet run likedpostsummary {typetalk-spacekey} -t {topickId} -from 2018-11-01 -to 2018-11-30 -l table
```

# 期間内のタグごとの投稿数
```
dotnet run talkssummary  {typetalk-spacekey} -t {topickId} -from 2018-11-01 -to 2018-11-30 -l table
```