# NukoTween

NukoTweenはUdonで実装されたTweenアニメーションエンジンです。VRChatで使用する事を目的としています。

## 導入方法

### 前提条件
以下のパッケージが導入されている事が前提となります。
- Unity 2018.4.20f1 
- [VRChat SDK3 - World](https://vrchat.com/home/download)
- [UdonSharp](https://github.com/MerlinVR/UdonSharp)

### インストール
1. [NukoTweenのリリースページ](https://github.com/nukora/NukoTween/releases)から最新版のunitypackageをダウンロードします。
2. VRCSDK3、UdonSharpが導入されているプロジェクトにダウンロードしたunitypackageをインポートします。

### 使用方法
1. Assets/NukoTweenディレクトリの中にあるNukoTweenEngineプレハブをヒエラルキーに配置します。
2. ヒエラルキー上にTweenさせたいオブジェクトを作成します。
3. 作成したオブジェクトにUdonBehaviorをアタッチし、以下のようなスクリプトを書きます。
```
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TweenCube : UdonSharpBehaviour
{
    public NukoTween.NukoTweenEngine tween;

    public override void Interact()
    {
        tween.LocalMoveTo(gameObject, new Vector3(1f, 0.5f, 0f), 1f, 0f, tween.EaseInOutCubic, false);
    }
}
```
4. インスペクターのtween欄にヒエラルキー上のNukoTweenEngineをアタッチします。
5. シーン再生後Trigger Interactボタンを押下し、アニメーションが再生される事を確認します。

## 機能一覧

編集中

## ライセンス

このプログラムにはMITライセンスが適用されます。

This software is released under the MIT License, see LICENSE.
