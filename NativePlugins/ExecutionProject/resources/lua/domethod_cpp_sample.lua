-- C++���̊֐���Lua�ŌĂяo��
-- �T���v���p��Lua

-- C���ꑤn�̊֐����Ăяo�����\�b�h


-- ���`�⊮
-- �⊮���ʂ�print�ŏo��.
-- Linear_print=function()
--     while t<= 1.0 do
--         print(Linear(a,b,t));
--         t=t+0.1;
--     end
-- end
a = 10.0;
b = 30.0;
t = 0.0;
while t<= 1.0 do
            print(Linear(a,b,t));
            t=t+0.1;
        end

function print_noarg()

    print("No argment lua function.");
    func_cpp();
end