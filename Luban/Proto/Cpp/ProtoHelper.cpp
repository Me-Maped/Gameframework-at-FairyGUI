#include "ProtoHelper.h"

#include "Common.pb.h"
#include "Msg.pb.h"
ProtoHelper ProtoHelper::Inst;
google::protobuf::Message *ProtoHelper::GetProtoMsg(PROTO_TYPE type)
{
    if (pProtoMap.empty()) return Factory(type);
    auto it = pProtoMap.find(type);
    if(it->second == nullptr) return Factory(type);
    return it->second;
}

google::protobuf::Message *ProtoHelper::Factory(PROTO_TYPE type)
{
    google::protobuf::Message *result;
    switch (type)
    {
		
        case PROTO_TYPE::ErrorAck:
            result = new pb::ErrorAck();
            break;
		
        case PROTO_TYPE::PingReq:
            result = new pb::PingReq();
            break;
		
        case PROTO_TYPE::PingAck:
            result = new pb::PingAck();
            break;
		
        case PROTO_TYPE::TestNtf:
            result = new pb::TestNtf();
            break;
		
        case PROTO_TYPE::SyncPid:
            result = new pb::SyncPid();
            break;
		
        case PROTO_TYPE::SyncPlayers:
            result = new pb::SyncPlayers();
            break;
		
        case PROTO_TYPE::Player:
            result = new pb::Player();
            break;
		
        case PROTO_TYPE::Position:
            result = new pb::Position();
            break;
		
        case PROTO_TYPE::MovePackage:
            result = new pb::MovePackage();
            break;
		
        case PROTO_TYPE::BroadCast:
            result = new pb::BroadCast();
            break;
		
        case PROTO_TYPE::Talk:
            result = new pb::Talk();
            break;
		
    }
    if(result != nullptr)
        pProtoMap.insert(std::pair<PROTO_TYPE,google::protobuf::Message*>(type,result));
    return nullptr;
}