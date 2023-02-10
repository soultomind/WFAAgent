HashMap = function () {
    this._map = new Array();
};

HashMap.prototype = {
    put: function (key, value) {
        this._map[key] = value;
    },
    get: function (key) {
        return this._map[key];
    },
    clear: function () {
        this._map = new Array();
    },
    keys: function () {
        var array = new Array();
        for (item in this._map) {
            array.push(item);
        }
        return array;
    }
};
function WFAAgent(config) {

    this._bWsInitialize = false;

    this._bWsAutoConnect = false;
    this._bWsSecure = false;
    this._strIpAddress = "127.0.0.1";
    this._strWsPort = 33000;
    this._strWssPort = 33001;
    this._strBehavior = "WFAAgent";
    this._evtWsOnOpenEventHandler = null;
    this._evtWsOnErrorEventHandler = null;
    this._evtWsOnMessageEventHandler = null;
    this._evtWsOnCloseEventHandler = null;

    this._evtExceptionEventHandler = null;
    this._evtProcessStartedEventHandler = null;
    this._evtProcessExitedEventHandler = null;

    this._evtTcpServerListenEventHandler = null;
    this._evtTcpServerAcceptClientEventHandler = null;
    this._evtTcpClientDataReceivedEventHandler = null;
    this._evtTcpClientDisconnectedEventHandler = null;

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
            this._strWsPort = config.wsPort;
        }

        if (config.hasOwnProperty("wssPort")) {
            this._strWssPort = config.wssPort;
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

WFAAgent.prototype.setExceptionEventHandler = function (evt) {
    this._evtExceptionEventHandler = evt;
};

WFAAgent.prototype.setProcessStartedEventHandler = function (evt) {
    this._evtProcessStartedEventHandler = evt;
};

WFAAgent.prototype.setProcessExitedEventHandler = function (evt) {
    this._evtProcessExitedEventHandler = evt;
};

WFAAgent.prototype.setTcpServerListenEventHandler = function (evt) {
    this._evtTcpServerListenEventHandler = evt;
}

WFAAgent.prototype.setTcpServerAcceptClientEventHandler = function (evt) {
    this._evtTcpServerAcceptClientEventHandler = evt;
};

WFAAgent.prototype.setTcpClientDataReceivedEventHandler = function (evt) {
    this._evtTcpClientDataReceivedEventHandler = evt;
};

WFAAgent.prototype.setTcpClientDisconnectedEventHandler = function (evt) {
    this._evtTcpClientDisconnectedEventHandler = evt;
};

WFAAgent.prototype.OnClientEventCallbackHandler = function (e) {
    if (e.data instanceof Blob) {
        // AppBinaryData
        var blob = e.data;

        var fileReader = new FileReader();
        var parent = this;
        fileReader.onload = function (event) {
            var arrayBuffer = event.target.result;
            window.console.log("ArrayBuffer.ByteLength=" + arrayBuffer.byteLength);
            //var dataview = new DataView(arrayBuffer);
            //var answer = dataview.getFloat64(0);

            parent.OnTcpClientDataReceived({
                type: "AgentBinaryData",
                isBase64: false,
                appBinaryData: arrayBuffer
            });
        };
        fileReader.readAsArrayBuffer(blob);
    } else {
        try {
            var data = JSON.parse(e.data);
            switch (data.eventName) {
                case "Exception":
                    if (this._evtExceptionEventHandler != null) {
                        this._evtExceptionEventHandler(data);
                    }
                    break;
                case "ProcessStarted":
                    if (this._evtProcessStartedEventHandler != null) {
                        this.wsAddProcessStartedInfo(data);
                        this._evtProcessStartedEventHandler(data);
                    }
                    break;
                case "ProcessExited":
                    if (this._evtProcessExitedEventHandler != null) {
                        this.wsRemoveProcessStartedInfo(data);
                        this._evtProcessExitedEventHandler(data);
                    }
                    break;
                case "TcpServerListen":
                    if (this._evtTcpServerListenEventHandler != null) {
                        this._evtTcpServerListenEventHandler(data);
                    }
                    break;
                case "TcpServerAcceptClient":
                    if (this._evtTcpServerAcceptClientEventHandler != null) {
                        this._evtTcpServerAcceptClientEventHandler(data);
                    }
                    break;
                case "TcpClientDataReceived":
                    this.OnTcpClientDataReceived(data);
                    break;
                case "TcpClientDisconnectEvent":
                    if (this._evtTcpClientDisconnectedEventHandler != null) {
                        this._evtTcpClientDisconnectedEventHandler(data);
                    }
                    break;
            }
        } catch (e) {
            window.console.error(e.stack);
            window.console.error(e.data);
        }
    }
};

WFAAgent.prototype.isWsConnect = function () {
    return this._bWsConnect;
};

WFAAgent.prototype.wsConnect = function () {
    try {
        var url = "";
        if (this._bWsSecure) {
            url = "wss://" + this._strIpAddress + ":" + this._strWssPort + "/" + this._strBehavior;
        } else {
            url = "ws://" + this._strIpAddress + ":" + this._strWsPort + "/" + this._strBehavior;
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
            parent._bWsConnect = false;
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

WFAAgent.prototype.wsDisconnect = function () {
    if (this._bWsConnect) {
        this._WebSocket.close();
    }
};

WFAAgent.prototype.wsSend = function (message) {
    if (this._bWsConnect) {
        this._WebSocket.send(message);
    }
}

WFAAgent.prototype.wsProcessStartData = function (startInfo, executeData) {
    if (this._bWsConnect) {
        var sendData = {
            eventName: "ProcessStartData",
            data: {

            }
        };

        if (typeof startInfo !== "undefined") {
            sendData.data.startInfo = startInfo;
        }

        if (typeof executeData !== "undefined") {
            sendData.data.executeData = executeData;
        }

        this.wsSend(JSON.stringify(sendData));
    }
}

WFAAgent.prototype.wsProcessEventData = function (eventDataName, eventData) {
    if (this._bWsConnect) {
        var sendData = {
            eventName: "ProcessEventData",
            data: {
                eventDataName: eventDataName,
                eventData: eventData
            }
        };

        this.wsSend(JSON.stringify(sendData));
    }
}

WFAAgent.prototype.OnTcpClientDataReceived = function (data) {
    // window.console.info("OnTcpClientDataReceived=" + JSON.stringify(data));

    switch (data.type) {
        case "AcceptClient":
            // 내부 로직
            break;

        case "AgentStringData":
            if (this._evtTcpClientDataReceivedEventHandler != null) {
                this._evtTcpClientDataReceivedEventHandler(data);
            }
            break;
        case "AgentBinaryData":
            if (this._evtTcpClientDataReceivedEventHandler != null) {
                this._evtTcpClientDataReceivedEventHandler(data);
            }
            break;
    }
};

WFAAgent.prototype.wsAddProcessStartedInfo = function (data) {

};

WFAAgent.prototype.wsRemoveProcessStartedInfo = function (data) {

};