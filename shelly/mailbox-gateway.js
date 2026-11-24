let targetAddr = "ThisIsTheMacAddressOfTheDoor/WindowsBlueetoothShellyDeviceThaIsOntheMailbox";
let endpoint = "https://WebAddressTBD.azurewebsites.net/api/MailboxWatcher/shelly";

function toHex(bytes) {
  let hex = "";
  for (let i = 0; i < bytes.length; i++) {
    let byte = bytes.charCodeAt(i).toString(16);
    if (byte.length < 2) byte = "0" + byte;
    hex += byte;
  }
  return hex;
}

function onPostComplete(result, errorCode, errorMessage) {
  if (errorCode !== 0) {
    print("HTTP.POST failed: " + errorMessage);
    return;
  }
  print("HTTP.POST status: " + result.code);
}

BLE.Scanner.Subscribe(function (event, result) {
  if (event !== BLE.Scanner.SCAN_RESULT || !result || result.addr !== targetAddr) return;
  if (!result.service_data || !result.service_data["fcd2"]) return;

  Shelly.call("HTTP.POST", {
    url: endpoint,
    body: JSON.stringify({
      deviceId: targetAddr,
      fcd2Hex: toHex(result.service_data["fcd2"])
    }),
    headers: {
      "Content-Type": "application/json"
    }
  }, onPostComplete);
});

print("SCRIPT STARTED");
let scan = BLE.Scanner.Start({
  duration_ms: BLE.Scanner.INFINITE_SCAN,
  active: false
});
print("SCAN START RESULT:", scan);
