package utils

import (
	"zinx/demo_server/pb"
	"zinx/demo_server/zrouter"
	"zinx/lib/ziface"
)

func RegisterRouter(s *ziface.IServer) {
	(*s).AddRouter(uint32(pb.Const_PingReq), &zrouter.PingRouter{})
}
