using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ���}���Ȧb����Ҧ��ɤ~���ӳQ�����ܪ��a���W
 */

public class PlayerInSoul : MonoBehaviour
{
    /*
     * ������� �HID�̧Ǫ��
     * 0�M�̡G�J���Y�e�{����A�C 
	 * 1�k�ӡG�^��P�Q�^��ɶ���֢����H�C	
     * 2���q�G�i�M�����W�Ҧ����a�t�����A�A�ޯ�N�o�ɶ����Q��C
     * 3�Q�H�G�Q���l���Ҧ��M�̩Ҩ��ˮ`�P�t�����A�A�ޯ�N�o���Q��C
     */
    public float ID;    //���a�b����������
    public bool soulOut;    //�O�_����
    public bool poison1;
    public bool poison2;
    void Start()
    {
        if (ID == 0) soulOut = true;
        else soulOut = false;
        poison1 = false;
        poison2 = false;
        GameManager.Instance.playerInSoulList.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
