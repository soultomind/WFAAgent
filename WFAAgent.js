
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
        }

        this._WebSocket.onclose = function (e) {
            if (parent._evtWsOnCloseEventHandler != null) {
                parent._evtWsOnCloseEventHandler(e);
            }
        }

        this._bWsConnect = true;
    } catch (e) {
        error("WebSocket 생성 실패");
        error(e.stack);
    }
};

