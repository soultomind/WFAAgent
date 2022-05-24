## WFAAgent
WFA(Windows Form Application) Agent

### 개발환경   
+ .NET Framework 4.5
+ Visual Studio 2015 Professional

# 프로그램 설명
웹 브라우저에서 웹 소켓을 통하여 .NET Framework C# WinForm 으로 개발된 프로그램을 실행시켜주는 프로그램

1) 웹 소켓 서버를 통하여 클라이언트(웹 브라우저)에서 보낸 이벤트 메시지를 받아 클라이언트 프로그램을 실행
2) 1 이후에는 양방향 통신으로 웹 브라우저 <--> 클라이언트 프로그램 간의 데이터 송수신 가능

# 
<strong>WFAAgent</strong>는 크게 2개의 프로세스로 구분됩니다.
+ 모니터링(Monitoring)   
모니터링(Monitoring)은 윈도우즈 서비스로 동작하는 프로그램은 아니지만 서버(Server) 프로세스를 감지하여 서버(Server) 프로세스가 종료 되었을때 다시 실행해줍니다.
+ 서버(Server)   
실제 웹 브라우저와 웹 소켓을 통하여 WFA 프로그램을 실행시켜주며 그 이후에는 웹 브라우저와 클라이언트(WFA 프로그램)간에 데이터 송수신을 담당합니다. WebSocket 서버, TcpSocket 서버 내장을 하고 있으며   
WebSocket 서버 : 브라우저와 통신을 담당.   
TcpSocket 서버 : 클라이언트 프로그램과 통신을 담당.


사용 라이브러리

### 1) SuperSocket
https://github.com/kerryjiang/SuperWebSocket

### 2) log4net
https://github.com/apache/logging-log4net

### 3) Newtonsoft.Json
https://github.com/JamesNK
