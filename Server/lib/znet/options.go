package znet

import (
	"zinx/lib/ziface"
)

type Option func(s *Server)

// 只要实现Packet 接口可自由实现数据包解析格式，如果没有则使用默认解析格式
func WithPacket(pack ziface.IDataPack) Option {
	return func(s *Server) {
		s.packet = pack
	}
}
