## Description
This plugins contains a set of utilitarian actions:
- **ChangeDisplayResolution**: You specify the resolution to which you desire to change. The first time that the action is executed is going to set the resolution used during configuration, the second time is going to turn back to the original resolution.
This action is particularly useful for folks that use ultra wide monitors and need to share content with others, because most people use a different aspect ratio you can force your monitor to switch to the desire aspect ratio by switching the screen resolution.


## Contributions

This plugin was made using the [FritzAndFriends stream deck toolkit](https://github.com/FritzAndFriends/StreamDeckToolkit). This is a unofficial C# toolkit.

### How to build:
To build the solution use `dotnet build [solution name]` after building, a script is going to run to register the plugin binaries with stream deck. If you see no updates in stream deck
close Stream Deck and open it again.

### How to debug:
To debug an action you can attach your debugger to the action's process. To debug the `property_inspector` you need to first enable remote debugging in the stream deck, look at the section [Debugging in the official docs](https://developer.elgato.com/documentation/stream-deck/sdk/create-your-own-plugin/#Debugging).
Once stream deck was configured to run a remote debugger, then you need to attach chrome's remote dev-tools to the debugger. Type in chrome's navigation bar `chrome://inspect` and if stream deck is already running you should see the different actions that you can inspect, if not use add the address to the remote debugger
in `Discover network targets`.

## References

* [Plugin Homepage](https://github.com/FritzAndFriends/StreamDeckToolkit)
* [Stream Deck Page][Stream Deck]
* [Stream Deck SDK Documentation][Stream Deck SDK]

<!-- References -->
[Stream Deck]: https://www.elgato.com/en/gaming/stream-deck "Elgato's Stream Deck landing page for the hardware, software, and SDK"
[Stream Deck software]: https://www.elgato.com/gaming/downloads "Download the Stream Deck software"
[Stream Deck SDK]: https://developer.elgato.com/documentation/stream-deck "Elgato's online SDK documentation"
[Style Guide]: https://developer.elgato.com/documentation/stream-deck/sdk/style-guide/ "The Stream Deck SDK Style Guide"
[Manifest file]: https://developer.elgato.com/documentation/stream-deck/sdk/manifest "Definition of elements in the manifest.json file"
