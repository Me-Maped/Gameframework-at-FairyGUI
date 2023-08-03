#pragma once
#include "ProtoType.h"
#include <google/protobuf/message.h>

class ProtoHelper
{
private:
    std::map<PROTO_TYPE,google::protobuf::Message *> pProtoMap;
    google::protobuf::Message* Factory(PROTO_TYPE type);
public:
	static ProtoHelper Inst;
    google::protobuf::Message* GetProtoMsg(PROTO_TYPE type);
};