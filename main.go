package main

import (
	"fmt"
	"log"
	"net"
	"os"
	"playerctl-cvr/socket"
)

func main() {
	//parse port from cmd
	socket.ParseFlag()

	// Listen for incoming connections.
	l, err := net.Listen(socket.CONN_TYPE, fmt.Sprintf("%s:%s", socket.CONN_HOST, socket.CONN_PORT))
	if err != nil {
		fmt.Println("Error listening:", err.Error())
		os.Exit(1)
	}
	// Close the listener when the application closes.
	defer l.Close()
	log.Printf("Listening on %s:%s", socket.CONN_HOST, socket.CONN_PORT)
	for {
		// Listen for an incoming connection.
		conn, err := l.Accept()
		if err != nil {
			fmt.Println("Error accepting: ", err.Error())
			os.Exit(1)
		}
		// Handle connections in a new goroutine.
		go socket.HandleConnection(conn)
	}
}
