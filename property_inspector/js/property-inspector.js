// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
  uuid = null,
  inInfo = null,
  actionInfo = {},
  settingsModel = {
    Width: 0,
    Heigth: 0
  };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
    uuid = inUUID;
    actionInfo = JSON.parse(inActionInfo);
    inInfo = JSON.parse(inInfo);
    console.log(inInfo);
    websocket = new WebSocket('ws://localhost:' + inPort);

    //initialize values
    if (actionInfo.payload.settings.settingsModel) {
        settingsModel.Width = actionInfo.payload.settings.settingsModel.Width;
        settingsModel.Heigth = actionInfo.payload.settings.settingsModel.Heigth;
    }

    document.getElementById('widthValue').value = settingsModel.Width;
    document.getElementById('heigthValue').value = settingsModel.Heigth;

    websocket.onopen = function () {
        var json = { event: inRegisterEvent, uuid: inUUID };
        // register property inspector to Stream Deck
        websocket.send(JSON.stringify(json));
    };

    websocket.onmessage = function (evt) {
        // Received message from Stream Deck
        var jsonObj = JSON.parse(evt.data);
        var sdEvent = jsonObj['event'];
        switch (sdEvent) {
          case "didReceiveSettings":
                if (jsonObj.payload.settings.settingsModel.Width) {
                    settingsModel.Width = jsonObj.payload.settings.settingsModel.Width;
                    document.getElementById('widthValue').value = settingsModel.Width;
                }

                if (jsonObj.payload.settings.settingsModel.Heigth) {
                    settingsModel.Heigth = jsonObj.payload.settings.settingsModel.Heigth;
                    document.getElementById('heigthValue').value = settingsModel.Heigth;
                }
                break;
          default:
            break;
        }
    };
}

const setSettings = (value, param) => {
    if (websocket) {
        settingsModel[param] = value;
        var json = {
              "event": "setSettings",
              "context": uuid,
              "payload": {
                "settingsModel": settingsModel
              }
        };
        websocket.send(JSON.stringify(json));
    }
};

