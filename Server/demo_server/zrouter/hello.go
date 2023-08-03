package zrouter

import (
	"zinx/demo_server/pb"
	"zinx/lib/ziface"
	"zinx/lib/zlog"
	"zinx/lib/znet"
)

type HelloZinxRouter struct {
	znet.BaseRouter
}

// Handle HelloZinxRouter Handle
func (*HelloZinxRouter) Handle(request ziface.IRequest) {
	zlog.Debug("Call HelloZinxRouter Handle")
	//先读取客户端的数据，再回写ping...ping...ping
	zlog.Debug("recv from client : msgId=", request.GetMsgID(), ", data=", string(request.GetData()))

	err := request.GetConnection().SendBuffMsg(pb.Const_PingAck, []byte("Hello Zinx Router V0.10"))
	if err != nil {
		zlog.Error(err)
	}
}
