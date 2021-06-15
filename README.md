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
```C#
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

## メソッド一覧

全ての機能は`NukoTweenEngine`クラスのインスタンスメソッドとして実装されています。  
引数には以下の値を設定します。
- `target` 操作する対象
- `to` 操作後の状態
- `duration` 操作にかける時間(秒)
- `delay` 操作の開始を遅らせる時間(秒)
- `easeId` 使用する[イージング関数](#イージング関数)
- `relative` 現在の状態から相対的に変化させるかどうか

### Transform
#### `LocalMoveTo`
```C#
LocalMoveTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
```
ターゲットのLocalPositionを指定した位置に変更します。

#### `MoveTo`
```C#
MoveTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
```
ターゲットのPositionを指定した位置に変更します。

#### `LocalScaleTo`
```C#
LocalScaleTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
```
ターゲットのLocalScaleを指定した大きさに変更します。

#### `LocalRotateTo`
```C#
LocalRotateTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
```
ターゲットのLocalRotateを指定した角度に変更します。  
角度はオイラー角で指定します。

#### `LocalRotateQuaternionTo`
```C#
LocalRotateQuaternionTo(GameObject target, Quaternion to, float duration, float delay, int easeId, bool relative)
```
ターゲットのLocalRotateを指定した角度に変更します。  
角度はクォータニオンで指定します。

#### `RotateTo`
```C#
RotateTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
```
ターゲットのRotateを指定した角度に変更します。  
角度はオイラー角で指定します。

#### `RotateQuaternionTo`
```C#
RotateQuaternionTo(GameObject target, Quaternion to, float duration, float delay, int easeId, bool relative)
```
ターゲットのRotateを指定した角度に変更します。  
角度はクォータニオンで指定します。

### RectTransform
#### `AnchorPosTo`
```C#
AnchorPosTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
```
ターゲットのAnchoredPositionを指定した位置に変更します。

### Graphic
#### `GraphicColorTo`
```C#
GraphicColorTo(Graphic target, Color to, float duration, float delay, int easeId)
```
ターゲットのColorを指定した色に変更します。

#### `GraphicFadeTo`
```C#
GraphicFadeTo(Graphic target, float to, float duration, float delay, int easeId)
```
ターゲットのColorの透明度を指定した値に変更します。

### Image
#### `FillAmountTo`
```C#
FillAmountTo(Image target, float to, float duration, float delay, int easeId)
```
ターゲットのFillAmountを指定した値に変更します。

### Text
#### `TextTo`
```C#
TextTo(Text target, string to, float duration, float delay, int easeId)
```
ターゲットのTextを操作し、文字送りアニメーションを行います。

### TextMeshPro
#### `TMPTextTo`
```C#
TMPTextTo(TextMeshProUGUI target, string to, float duration, float delay, int easeId)
```
ターゲットのTextを操作し、文字送りアニメーションを行います。

### AudioSource
#### `AudioFadeTo`
```C#
AudioFadeTo(AudioSource target, float to, float duration, float delay, int easeId)
```
ターゲットのVolumeを指定した音量に変更します。

### Material
#### `MaterialColorTo`
```C#
MaterialColorTo(Material target, string propertyName, Color to, float duration, float delay, int easeId)
```
マテリアルのColor型のプロパティを指定した色に変更します。

#### `MaterialFadeTo`
```C#
MaterialFadeTo(Material target, string propertyName, float to, float duration, float delay, int easeId)
```
マテリアルのColor型のプロパティの透明度を指定した値に変更します。

#### `MaterialVectorTo`
```C#
MaterialVectorTo(Material target, string propertyName, Vector4 to, float duration, float delay, int easeId)
```
マテリアルのVector型のプロパティを指定した値に変更します。

#### `MaterialFloatTo`
```C#
MaterialFloatTo(Material target, string propertyName, float to, float duration, float delay, int easeId)
```
マテリアルのFloat型のプロパティを指定した値に変更します。

#### `MaterialTexOffsetTo`
```C#
MaterialTexOffsetTo(Material target, string propertyName, Vector2 to, float duration, float delay, int easeId)
```
マテリアルのテクスチャ型のプロパティのOffsetを指定した値に変更します。

#### `MaterialTexTilingTo`
```C#
MaterialTexTilingTo(Material target, string propertyName, Vector2 to, float duration, float delay, int easeId)
```
マテリアルのテクスチャ型のプロパティのTilingを指定した値に変更します。

### Misc
#### `DelayedSetActive`
```C#
DelayedSetActive(GameObject target, bool active, float delay)
```
指定した時間後にGameObjectのSetActiveを変更します。

#### `DelayedCall`
```C#
DelayedCall(UdonSharpBehaviour target, string customEventName, float delay)
```
指定した時間後にSendCustomEventを実行します。

### Control Mathods
#### `Complete`
```C#
Complete(int tweenId)
```
動作中のtweenを即座に完了状態にします。

#### `Kill`
```C#
Kill(int tweenId)
```
動作中のtweenを現在の状態で中止します。

### Loop
登録したtweenがループするように設定します。  
loopsにはループ回数を指定し、-1を渡すと無限ループになります。

#### `LoopRestart`
```C#
LoopRestart(int tweenId, int loops)
```
ループする際、前回の始点に値を戻してからアニメーションが再実行されます。

#### `LoopReverse`
```C#
LoopRestart(int tweenId, int loops)
```
ループする際、前回の終点を始点として、前回の始点へ戻るようなアニメーションを繰り返します。

#### `LoopIncremental`
```C#
LoopRestart(int tweenId, int loops)
```
ループする際、前回の終点を始点として、前回と同じアニメーションを実行します。

## イージング関数

以下のイージング関数を使用できます。  
`NukoTweenEngine`クラスの読み取り専用のインスタンス変数として定義されていますので、メソッド呼び出し時はこちらを使用してください。

- EaseLinear
- EaseInSine
- EaseOutSine
- EaseInOutSine
- EaseInQuad
- EaseOutQuad
- EaseInOutQuad
- EaseInCubic
- EaseOutCubic
- EaseInOutCubic
- EaseInQuart
- EaseOutQuart
- EaseInOutQuart
- EaseInQuint
- EaseOutQuint
- EaseInOutQuint
- EaseInExpo
- EaseOutExpo
- EaseInOutExpo
- EaseInCirc
- EaseOutCirc
- EaseInOutCirc
- EaseInBack
- EaseOutBack
- EaseInOutBack
- EaseInElastic
- EaseOutElastic
- EaseInOutElastic
- EaseInBounce
- EaseOutBounce
- EaseInOutBounce

## プロパティ

エンジンの動作をカスタマイズします。  
`NukoTweenEngine`をアタッチしたオブジェクトのインスペクターから変更可能です。

### Simultaneous Size
tweenを同時に実行できる数を指定します。これには待機中のtweenも含まれます。

## ライセンス

このプログラムにはMITライセンスが適用されます。

This software is released under the MIT License, see LICENSE.
