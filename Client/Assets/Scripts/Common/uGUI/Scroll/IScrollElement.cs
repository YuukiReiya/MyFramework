using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ScrollView - Cell�̎w��͂������Ȃ��i�[�Ă�v���n�u�ς��Ă����삷����x�j
 * Cell - ��p�ŗp�ӂ��Ă������i�j
 */
namespace uGUI
{
    public interface IScrollElement<T>
    {
        void Setup(T data);
    }
}