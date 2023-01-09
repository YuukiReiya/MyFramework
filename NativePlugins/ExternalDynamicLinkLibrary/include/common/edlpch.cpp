#include "common/edlpch.hpp"
#include<Windows.h>
#include<tchar.h>

DLL_EXPORT void UNITY_API MsgBoxWin32(DWORD errorcode)
{
    if (errorcode == NO_ERROR)return;

    LPVOID lpMsgBuf;
    FormatMessage(
        FORMAT_MESSAGE_ALLOCATE_BUFFER                                                   //  �e�L�X�g�̃��������蓖�Ă�v������
        | FORMAT_MESSAGE_FROM_SYSTEM                                                       //  �G���[���b�Z�[�W��Windows���p�ӂ��Ă�����̂��g�p
        | FORMAT_MESSAGE_IGNORE_INSERTS,                                                   //  ���̈����𖳎����ăG���[�R�[�h�ɑ΂���G���[���b�Z�[�W���쐬����
        NULL, errorcode, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),    //  ������w��
        (LPTSTR)&lpMsgBuf,                                                                                  //  ���b�Z�[�W�e�L�X�g���ۑ������o�b�t�@�ւ̃|�C���^
        0,
        NULL);

    MessageBox(NULL ,static_cast<LPCTSTR>(lpMsgBuf), _TEXT("�G���["), MB_OK | MB_ICONINFORMATION);
    LocalFree(lpMsgBuf);
}
