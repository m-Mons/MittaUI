<h1 align="center">MittaUI</h1>

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)

UnityにおけるuGUI周りをハッカソンへ向けて行うための基盤を提供するライブラリです。

## 概要

### 本ライブラリの目的
- UnityのデフォルトUIを発展させ、ハッカソン等の短期開発へ大きなバリューを出すことを目的としたUI基盤になります

### 特徴
- 基本的なUI Prefabの構成が派生式となっており、プロジェクトの要望次第で柔軟な変更が可能
- Hoge
- Fuga

## セットアップ

### 要件
* Unity 2022.3 以上
* 動作のためにUniTask, R3が必須

### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下を入力してインストール
   * https://github.com/m-Mons/MittaUI.git
   

<p align="center">
  <img width="50%" src="https://github.com/m-Mons/MittaUI/assets/64365341/3e4e5aba-0322-4503-808b-fe43c89bce39.png">
</p>

あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記します。

```json
{
    "dependencies": {
        "com.mitta.mitta-ui": "https://github.com/m-Mons/MittaUI.git
        "
    }
}
```

バージョンを指定したい場合には以下のように記述します。

* https://github.com/https://github.com/m-Mons/MittaUI.git
#1.0.0

## 本リポジトリで行っていること
* UIの基盤提供
* Unity Screen Navigatorを用いたModalのPopUp表示機能
* 簡単なUIアニメーション
* サンプルとなるUI画面の提供

## リポジトリ運用
1. Issue&プロジェクトの設計方針&既存実装を確認
- 新規作成, 提案して作成, 基盤などのリファインを募集
- Issueが立っていない基盤の実装でも、Issueを立てて作業OKです
2. 完成したらプルリクを出してください！
3. PRレビューしてから、マージ（アプルーブ1以上！）

## ライセンス
本ソフトウェアはMITライセンスで公開しています。  
ライセンスの範囲内で自由に使っていただけますが、著作権表示とライセンス表示が必須となります。

* https://github.com/m-Mons/MittaUI.git
