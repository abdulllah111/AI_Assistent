from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class PromtRequest(_message.Message):
    __slots__ = ["message", "userid"]
    MESSAGE_FIELD_NUMBER: _ClassVar[int]
    USERID_FIELD_NUMBER: _ClassVar[int]
    message: str
    userid: str
    def __init__(self, message: _Optional[str] = ..., userid: _Optional[str] = ...) -> None: ...

class PromtResponse(_message.Message):
    __slots__ = ["response", "userid"]
    RESPONSE_FIELD_NUMBER: _ClassVar[int]
    USERID_FIELD_NUMBER: _ClassVar[int]
    response: str
    userid: str
    def __init__(self, response: _Optional[str] = ..., userid: _Optional[str] = ...) -> None: ...
