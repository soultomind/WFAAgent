## WFAAgent .NET Framework 4.5
WFA(Windows Form Application) Agent

# 프로그램 설명
웹 브라우저에서 웹 소켓을 통하여 .NET Framework C# WinForm 으로 개발된 프로그램을 실행시켜주는 프로그램

1) 웹 소켓 서버를 통하여 클라이언트(웹 브라우저)에서 보낸 이벤트 메시지를 받아 클라이언트 프로그램을 실행
2) 반대로 1)에서 실행한 클라이언트 프로그램의 데이터를 받아 웹 블라이언트(웹 브라우저)로 데이터를 보내준다.

# 
<strong>WFAAgent</strong>는 WebSocket 서버, TcpSocket 서버 내장을 하고 있으며   
WebSocket 서버 : 브라우저와 통신을 담당.   
TcpSocket 서버 : 클라이언트 프로그램과 통신을 담당.


사용 라이브러리

### 1) SuperSocket
https://github.com/kerryjiang/SuperWebSocket

### 2) log4net
https://github.com/apache/logging-log4net

### 3) Newtonsoft.Json
https://github.com/JamesNK