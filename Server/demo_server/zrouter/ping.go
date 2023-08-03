package zrouter

import (
	"zinx/demo_server/pb"
	"zinx/lib/ziface"
	"zinx/lib/zlog"
	"zinx/lib/znet"

	"github.com/golang/protobuf/proto"
)

// PingRouter ping test 自定义路由
type PingRouter struct {
	znet.BaseRouter
}

// Handle Ping Handle
func (*PingRouter) Handle(request ziface.IRequest) {

	pingReq := &pb.PingReq{}

	if err := proto.Unmarshal(request.GetData(), pingReq); err != nil {
		zlog.Error(err)
		return
	}

	zlog.Debug("Call PingRouter Handle")
	//先读取客户端的数据，再回写ping...ping...ping
	zlog.Debug("recv from client : msgId=", request.GetMsgID(), ", CmdCode=", pingReq.CmdCode, ",ProtocolSwitch=", pingReq.ProtocolSwitch)

	msg := &pb.PingAck{Code: pingReq.CmdCode}
	data, marshalErr := proto.Marshal(msg)
	if marshalErr != nil {
		zlog.Error(marshalErr)
		return
	}

	err := request.GetConnection().SendBuffMsg(pb.Const_PingAck, data)
	if err != nil {
		zlog.Error(err)
	}

	//test ErrorAck
	if pingReq.CmdCode <= 0 {
		errMsg := &pb.ErrorAck{CmdId: pb.Const_PingAck, Msg: "test err"}
		errData, errMsgErr := proto.Marshal(errMsg)
		if errMsgErr != nil {
			zlog.Error(errMsgErr)
			return
		}
		ackErr := request.GetConnection().SendBuffMsg(pb.Const_ErrorAck, errData)
		if ackErr != nil {
			zlog.Error(ackErr)
		}
	}
}
