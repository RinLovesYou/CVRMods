# CVRMods
ChilloutVR mods by RinLovesYou

## [ProtonMediaControlFix]

### Requirements
* install [playerctl](https://github.com/altdesktop/playerctl)
* install [playerctl-tcp](https://github.com/RinLovesYou/playerctl-tcp)

`playerctl-tcp` is a lightweight little TCP socket made for this project. It will need to be running while you run the game for Media Control to function.

I recommend creating a user-space service for it.<br/>
user-space services are stored at `~/.config/systemd/user/`

Mine looks something like this

```service
[Unit]
Description=playerctl tcp bridge for proton

[Service]
WorkingDirectory=/home/sarah/go/bin
ExecStart=/home/sarah/go/bin/playerctl-tcp
Restart=always

[Install]
WantedBy=multi-user.target
```

You can then enable and start it with `systemctl --user enable playerctl-tcp` and `systemctl --user start playerctl-tcp`, assuming you saved the above as `playerctl-tcp.service`

Now the socket will always start when you login, and the media control buttons in CVR should work!

Alternatively, you can manually run the socket before you start the game.
