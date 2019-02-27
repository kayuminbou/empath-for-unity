# empath-for-unity
音声データから気分を数値化するEmpath APIをUnityで扱うためのサンプルコードです。  
Empathについては[こちら](https://webempath.net/lp-jpn/)を参照。

# 遊び方
## 事前準備
MainSceneを開き、EmpathManagerの以下の値を設定します。  
![PasteAppKey](./Assets/Images/image1.png)
* Subscription Key  
EmpathのWebコンソールからAPI Keyをコピーしてきて貼り付けましょう。
* Max Recording Time  
Empathに送信するための音声データを何秒間録音するか整数値で入力します。  
とはいってもEmpathの仕様上最大5秒までしか受け付けてくれないので6秒以上にしてもエラーで返ってくるだけです。
## 動かしてみる
"Rec Start"ボタンを押すと"Max Recording Time"で設定した秒数だけ音声入力を受け付けるので何か喋ってみましょう。  
画面中央に結果が表示されるはずです。  
![EmpathResult](./Assets/Images/image2.png)

# 環境
* Unity 2018.2.16f1
* Empath WebAPI v2
