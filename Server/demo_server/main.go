package main

import (
	"zinx/demo_server/utils"
	"zinx/lib/ziface"
	"zinx/lib/zlog"
	"zinx/lib/znet"
)

// 创建连接的时候执行
func DoConnectionBegin(conn ziface.IConnection) {
	zlog.Debug("DoConnectionBegin is Called ... ")

	//设置两个链接属性，在连接创建之后
	conn.SetProperty("Name", "Maped")
	zlog.Debug("Set conn Name done!")
}

// 连接断开的时候执行
func DoConnectionLost(conn ziface.IConnection) {
	//在连接销毁之前，查询conn的Name属性
	if name, err := conn.GetProperty("Name"); err == nil {
		zlog.Error("DisConnect Property Name = ", name)
	}

	zlog.Debug("DoConnectionLost is Called ... ")
}

func main() {
	//创建一个server句柄
	s := znet.NewServer()

	//注册链接hook回调函数
	s.SetOnConnStart(DoConnectionBegin)
	s.SetOnConnStop(DoConnectionLost)

	//注册路由
	utils.RegisterRouter(&s)

	//开启服务
	s.Serve()
}
