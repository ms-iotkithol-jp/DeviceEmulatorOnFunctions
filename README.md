# Device Emulator On Functions 
実デバイスめんどくさーい、じゃ、バーチャルマシン？それもたちあげんのめんどくせー。。。 
というあなたへ  
サーバーレスのAzure Functionsで気軽にAzure IoT Hubにセンサー情報を送信するデバイスエミュレータサンプルです。 
Azure Functionsで、タイマー起動の関数を作り、project.jsonを追加、run.csxを全部上書きして、 
Azure Iot Hubに"emulator"という名前のデバイスを登録し、接続文字列をiothubcsに設定します。 

```cs
    string iothubcs = "<< Device Connection String >>";
```

後は、時間間隔を5分から0分に変えて、できあがり。 
1分ごとに加速度、温度、湿度、時間がIoT Hubに60秒分まとめて送付します。 

```json
{"time":"2017...","accelx":0.01,"accely":-0.02,"accelz":-0.9812,"temp":27.563,"humidity":56.72}
```