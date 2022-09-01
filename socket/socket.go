package socket

import (
	"flag"
	"fmt"
	"log"
	"net"
	"os/exec"
)

var (
	CONN_HOST = "localhost"
	CONN_PORT = "8080"
	CONN_TYPE = "tcp"
)

func ParseFlag() {
	port := flag.String("port", "8080", "--port 8080")
	flag.Parse()
	CONN_PORT = *port
}

// Handles incoming requests.
func HandleConnection(conn net.Conn) {
	for {
		// Make a buffer to hold incoming data.
		buf := make([]byte, 1)
		// Read the incoming connection into the buffer.
		bufLen, err := conn.Read(buf)
		if err != nil {
			fmt.Println("Error reading:", err.Error())
			return
		}

		if bufLen == 1 {
			switch buf[0] {
			case 1:
				RunCmd("play-pause")
			case 2:
				RunCmd("next")
			case 3:
				RunCmd("previous")
			}
		}
	}
}

func RunCmd(command string) {
	cmd := exec.Command("playerctl", command)
	stdout, err := cmd.Output()

	if err != nil {
		log.Printf("Error executing playerctl: %e", err)
		return
	}

	log.Println(stdout)
}
