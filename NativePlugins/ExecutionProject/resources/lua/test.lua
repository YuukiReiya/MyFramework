-- Lua���̃O���[�o���ϐ���C���ꑤ�ŌĂяo����悤�Ƀe�X�g.
wndWidth=640;
wndHeight=480;
wndName="Marupeke";

-- C���ꑤ�Ŋ֐��Ăяo���̃e�X�g.
function add(a,b)
    return a+b;
end
function calc(a,b)
    return a+b,a-b,a*b,a/b;
end

function print_test(str)
    print(str);
end

-- �R���[�`���Ăяo���̃e�X�g.
function step()
coroutine.yield("�����͍L�ꂾ�����B")
coroutine.yield("�����Ȋ���䂪�������B")
coroutine.yield("�̂����ŗǂ��V�񂾂��Ƃ��v���o�����B")
end

----------------------------------------------------------
-- �e�[�u��
----------------------------------------------------------
-- �@ �e�[�u���Ɋ֐�������
tbl = {
    Name = "Hoge",
    Age = 20,
    Hometown = "�k�C��",
    TblFunc = function(str)
        print("tbl > "+str);
    end
};

-- �A �N���X�錾�{�I�u�W�F�N�g����������New�֐�
-- ���p:�u�C���X�^���X��2����2���̌ʃf�[�^�ɂȂ�܂���ˁBLua���̃e�[�u���ł��������Ƃ����悤�Ǝv������A�e�[�u�������̓x�ɍ��K�v������킯�ł��B�ł�����A�u�e�[�u�������֐��v��Lua���ɐݒ肷��K�v������܂��B�v
-- �ǂ������Ӗ��H ���rC����ő����Ă�����Lua�ł̏��������Ă��ƁH���C������B�B�B�m���Ɋ֐��Ȃ��Ɠ����e�[�u���������Ȃ��ƒ�`�ł��Ȃ������B
-- �Ăяo��Lua_State�ς���΃C���X�^���X���ʂŗ������������ǂ��ꂶ��_���Ȃ́H
-- ���Lua_State�Ŋ������������ꍇ�Ȃ�c���čl�������ǁALua���叉�S�҂����猻��ł̉e���ȂǕ����炸�œK�����o�Ȃ��B
-- �ЂƂ܂����̑̂������Ă�Ɖ��肵�ēǂݐi�߂�B
-- ��GoF�ł���Factor�H
tblFactor={
    New=function()
        return {
            Name = "def",
            Age = 0,
            Hometown = "def"
        };
    end
};
obj1 = tblFactor.New();
obj2 = tblFactor.New();

-- �������܂ł�C++�ł����N���X���ۂ��������˂��Ęb�B
-- �����o�ϐ����Q�Ƃ���N���X���\�b�h�̒�`�@�Ɋւ��āB

--[[
class={
    New=function()
        return {
            Name = "def",
            Age = 0,
            Hometown = "def",

            print = function()
                print(Name,Age,Hometown);
            end
        };
    end
};
obj3 = class.New();
-- �uprint�֐���V������`���āA�e�[�u�����̕ϐ����g���Ă���悤�Ɍ�����̂ł����A
-- ����͎c�O�ł������܂������܂���B
-- �G���[�ɂ͂Ȃ�Ȃ��̂ł����Aprint�֐����ŁuName, Age, Hometown�v�Ƃ���3�̕ϐ�����錾���ꂽ�Ƃ݂Ȃ����̂ŁA
-- �o�͌��ʂ͑S���unil�v�ɂȂ��Ă��܂��܂��B�v
obj3.print(); -- �v����Ɋ֐����̃��[�J���ϐ��Ƃ��ă����o�Ƃ͕ʂɐ錾����Ă��܂����߃_���炵���c�������ߓI�ɂ������Ă����B
]]

-- ���Ⴀ�ǂ�����ă����o�ɃA�N�Z�X����̂�.
-- �� �uthis�|�C���^�v�g���B
class = {
    New = function()
        return{
            Name = "def",
            Age = 0,
            Hometown = "def",
            
            print = function(this)
                print(this.Name,this.Age,this.Hometown);
            end
        };
    end
};
obj3 = class.New();
obj3.print(obj3);

-- �B�N���X���ۂ������������O���t�@�C���ɒu���ČĂяo��
-- �u�N���X���ۂ����Ƃ͂ł���悤�ɂȂ����킯�ł����A
-- ���񂱂��Lua�t�@�C���ɏ����̂͂������V���h�C�킯�ł��B
-- �O���t�@�C���ɂ��̐錾�����������L�q���āA�ʂ�Lua�t�@�C����
-- �͂�����Ăяo���Ďg����悤�ɂ���ƕ֗��ł��B
-- C����ł����u#include�v���~�����Ƃ����킯�ł��B�v
-- >������Ȃ����Ƃ͂Ȃ��A���╪����B

-- Lua�ɂ́urequire�v�Ƃ���C�����#include�ɑ������郉�C�u�����֐���
-- ����܂��B����̃N���X�錾������ClassData.lua�t�@�C���ɕۑ�����
-- �Ƃ��āA�g�����ł͎��̂悤��require�֐��ł����錾���܂��F

--[[
    -- �g�p��
    require("ClassData"); -- �ʃt�H���_�ɓ���Ă��鎞�́A�����܂ł̑��΃p�X���g���Ďw�� ��j"require("code/ClassData")"

    instance = Data.New();
    instance:print();
]]

--[[
    ���t�@�C�������w�肷��̂ł����u�g���q�����Ȃ��v���ɒ��ӂ��ĉ������B
    �t����ƕʋ����ɂȂ��Ă��܂��܂��B

    �������A����Lua�X�e�[�g�œ����t�@�C����require�����x�ɃR�[�h������܂��B
    ����͑�ʂ�Lua�t�@�C���������Ƃ��Ɏ������ƂȂ�܂��B
    C++�̏ꍇ�́A

    C����ł̏d���h�~
```cpp
    #ifndef IKD_OX_DATA_H
    #define IKD_OX_DATA_H

    ...

    #endif
```


    require�̏d���h�~(ClassData.lua)
```lua
if (IKD_OX_LUA_DATA_H == nil) {
    IKD_OX_LUA_DATA_H = 1

    Data = {
        ...
    };
}
```
�Ƃ����Ώ����K�v�ɂȂ邩������܂���B
> �܂��u����Lua�X�e�[�g�œ����t�@�C����require�����x�ɃR�[�h������܂��v���ꂪ�K�v�ɂȂ�P�[�X��
�@������Ȃ��̂ŕK�v�����킩��Ȃ��c
�@�ǁX���Ȃ��Ďv���Ă�B
]]

--
--�@

----------------------------------------------------------
--  Lua����C����̊֐����Ăяo��
----------------------------------------------------------

--------------------
-- �t���[
--------------------
--[[
  �@ C���ꑤ��Lua�X�e�[�g�Ɋ֐���������̂���1��


]]

--�@ C���ꑤ��Lua�X�e�[�g�Ɋ֐���������̂���1��
--[[
```lua
start = 10.0f;
goul = 30.0f;

--��Ԃ��悤
t = 0.0;
while t <= 1.0 do
    print(Linear(start, end, t));
    t = t + 0.1;
end
```
��L��Lua��Linear�֐������݂��Ȃ����߃G���[.

�������@�Ƃ��Ă͈ȉ�
�@ require�֐����g����Liniear�֐�����`����Ă���t�@�C����ǂݍ���
�A C���ꑤ�Ŋ֐���o�^����
�{���͇A�̂���.

��Lua�œǂݍ��߂�C����̊֐��^�͉��L��1��ނ�����
�߂�l��int�^�̈���lua_State*�^ - "�O���[�֐�(GlueFunction glue:��������)"�ƌĂ�
```cpp
int <���\�b�h��>(lua_State*){return 0;}
```

```cpp
// �Ăяo��
lua_State* = luaL_newstate();
// lua_State�Ɋ֐�"FuncC"��o�^
lua_register(L,"FuncC",&FuncC);
int FuncC(lua_State*L){ printf("hoge"); }
```

�ȉ��͈�����float�^�O�A�߂�l��float�^�ŕԂ������ꍇ�̊֐���o�^���邽�߂̕��@�B

lua���̌Ăяo���z�� - ���`�⊮�֐�
```lua
Linear(10,30,0.3);
```

C(++)���̒�`
```cpp
int Linear(lua_State*L)
{
    // ����
    float start = (float)lua_tonumber(L,1); // Arg1
    float end = (float)lua_tonumber(L,2); // Arg2
    float t = (float)lua_tonumber(L,3); // Arg3

    // �⊮�v�Z��.
    float res = (1.0 - t) * start + t * end;

    // �X�^�b�N�폜.
    lua_pop(L,lua_gettop(L));

    // �X�^�b�N�ɖ߂�l��ς�.
    lua_pushnumber(L,res);

    // �֐��̖߂�l�Ƃ��ăX�^�b�N�ɐς�(lua���ɒ�`�������֐���)�߂�l�̐���Ԃ��悤�ɂ���
    return 1;
}
```

]]

function entry()

-- �uLinear�v���\�b�h��C++���Œ�`����Ă��邱�Ƃ�z�肵�ČĂяo���B
-- Arg1:�J�n�n�_
-- Arg2:�I���n�_
-- Arg3:���ݒn�_
return Linear(10,30,0.3);
   --result = Linear(1,10,0.2);
   --print("LuaResult:",result);
--return result;

-- ��������������������������������������������������
-- C���ꑤ:int Linear(lua_State*)�̈����ƈ�v���Ȃ�.
-- Linear();



end

function entry2()

-- GlueFunction(��������)C�����`�̊֐��Ăяo���e�X�g
GlueFuncTest();

end