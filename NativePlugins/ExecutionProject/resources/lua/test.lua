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

-- �R���[�`���Ăяo���̃e�X�g.
function step()
coroutine.yield("�����͍L�ꂾ�����B")
coroutine.yield("�����Ȋ���䂪�������B")
coroutine.yield("�̂����ŗǂ��V�񂾂��Ƃ��v���o�����B")
end