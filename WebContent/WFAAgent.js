function WFAAgent(config) {

    this._bWsInitialize = false;

    this._bWsAutoConnect = false;
    this._bWsSecure = false;
    this._strIpAddress = "127.0.0.1";
    this._strPort = "33000";
    this._strBehavior = "WFAAgent";
    this._evtWsOnOpenEventHandler = null;
    this._evtWsOnErrorEventHandler = null;
    this._evtWsOnMessageEventHandler = null;
    this._evtWsOnCloseEventHandler = null;
    this._evtProcessStartedEventHandler = null;
    this._evtProcessExitedEventHandler = null;
    this._WebSocket = null;
    this._bWsConnect = false;

    this.wsInitialize(config);
}

WFAAgent.prototype.wsInitialize = function (config) {
    if (!this._bWsInitialize)
    {
        if (config.hasOwnProperty("wsAutoConnect")) {
            this._bWsAutoConnect = config.wsAutoConnect;
        }

        if (config.hasOwnProperty("wsSecure")) {
            this._bWsSecure = config.wsSecure;
        }

        if (config.hasOwnProperty("wsIpAddress")) {
            this._strIpAddress = config.wsIpAddress;
        }

        if (config.hasOwnProperty("wsPort")) {
            this._strPort = config.wsPort;
        }

        if (config.hasOwnProperty("wsBehavior")) {
            this._strBehavior = config.wsBehavior;
        }

        if (this._bWsAutoConnect) {
            this.wsConnect();
        }

        this._bWsInitialize = true;
    }
};

WFAAgent.prototype.error = function (msg) {
    window.console.error(msg);
};

WFAAgent.prototype.setWsOnOpenEventHandler = function (evt) {
    this._evtWsOnOpenEventHandler = evt;
};

WFAAgent.prototype.setWsOnErrorEventHandler = function (evt) {
    this._evtWsOnErrorEventHandler = evt;
};

WFAAgent.prototype.setWsOnMessageEventHandler = function (evt) {
    this._evtWsOnMessageEventHandler = evt;
};

WFAAgent.prototype.setWsOnCloseEventHandler = function (evt) {
    this._evtWsOnCloseEventHandler = evt;
};

WFAAgent.prototype.setProcessStartedEventHandler = function (evt) {
    this._evtProcessStartedEventHandler = evt;
};

WFAAgent.prototype.setProcessExitedEventHandler = function (evt) {
    this._evtProcessExitedEventHandler = evt;
};

WFAAgent.prototype.OnClientEventCallbackHandler = function (e) {
    if (event.data instanceof ArrayBuffer) {
        // TODO: Binary
    } else {
        var data = JSON.parse(e.data);
        switch (data.eventName) {
            case "ProcessStarted":
                if (this._evtProcessStartedEventHandler != null) {
                    this._evtProcessStartedEventHandler(data);
                }
                break;
            case "ProcessExited":
                if (this._evtProcessExitedEventHandler != null) {
                    this._evtProcessExitedEventHandler(data);
                }
                break;
        }
    }
};

WFAAgent.prototype.wsConnect = function () {
    try {
        var url = "";
        if (this._bWsSecure) {
            url = "wss://" + this._strIpAddress + ":" + this._strPort + "/" + this._strBehavior;
        } else {
            url = "ws://" + this._strIpAddress + ":" + this._strPort + "/" + this._strBehavior;
        }

        this._WebSocket = new WebSocket(url);
        var parent = this;
        this._WebSocket.onopen = function (e) {
            if (parent._evtWsOnOpenEventHandler != null) {
                parent._evtWsOnOpenEventHandler(e);
            }
        }

        this._WebSocket.onerror = function (e) {
            if (parent._evtWsOnErrorEventHandler != null) {
                parent._evtWsOnErrorEventHandler(e);
            }
        }

        this._WebSocket.onmessage = function (e) {
            if (parent._evtWsOnMessageEventHandler != null) {
                parent._evtWsOnMessageEventHandler(e);
            }

            parent.OnClientEventCallbackHandler(e);
        }

        this._WebSocket.onclose = function (e) {
            if (parent._evtWsOnCloseEventHandler != null) {
                parent._evtWsOnCloseEventHandler(e);
            }
        }

        this._bWsConnect = true;
    } catch (e) {
        error("Create failed WebSocket");
        error(e.stack);
    }
};

WFAAgent.prototype.wsSend = function (message) {
    if (this._bWsConnect) {
        this._WebSocket.send(message);
    }
}

/*
WFAAgent.prototype.wsExecute = function (fileName, data) {
    if (this._bWsConnect) {
        // 실행시 아규먼트 추가 여부 필요
        // 문서프로그램 같은 경우 예) Notepad.exe Test.exe 
        // 형식으로 보내기 때문임

        var sendData = {
            eventName = "ProcessStart"
        };
        if (arguments.length == 1) {
            sendData.data = {
                fileName = fileName
            };
        } else {
            sendData.data = data;
            sendData.data.fileName = fileName;
        }
    }
};
*/