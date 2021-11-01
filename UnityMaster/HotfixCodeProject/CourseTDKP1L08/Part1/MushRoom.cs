using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ILFramework.HotClass;


namespace ILFramework.HotClass
{
    public class MushRoom
    {
        GameObject mushroom;
        Transform poolTran;
        List<MushRoom> mushRoomPool;
        List<MushRoom> usedMushRoomPool;

        public MushRoom(GameObject mushRoom,Transform poolTran,List<MushRoom> mushRoomPool,List<MushRoom> usedMushRoomPool)
        {
            this.mushroom = mushRoom;
            this.mushroom.SetActive(false);
            this.poolTran = poolTran;
            this.mushRoomPool = mushRoomPool;
            this.usedMushRoomPool = usedMushRoomPool;
        }

        public void OutPool(Transform newProductTran)
        {   
            //Debug.Log("执行了OutPool");

            CourseTDKP1L08Part1.curCount++;
            mushroom.SetActive(true);
            mushroom.transform.SetParent(newProductTran);
            mushroom.transform.localPosition = Vector3.zero;
            CourseTDKP1L08Part1.action += Move;

            mushRoomPool.Remove(this);
            usedMushRoomPool.Add(this);
        }

        public void EnterPool()
        {
            CourseTDKP1L08Part1.curCount--;
            CourseTDKP1L08Part1.action -= Move;
            mushroom.SetActive(false);
            mushroom.transform.SetParent(poolTran);
            mushroom.transform.localPosition = Vector3.zero;
            usedMushRoomPool.Remove(this);
            mushRoomPool.Add(this);

        }

        void Move()
        {   
            //Debug.Log("执行了Move");
            mushroom.transform.Translate(Vector3.down * CourseTDKP1L08Part1.Speed * Time.deltaTime);
            if(mushroom.transform.localPosition.y < -1200f)
            {
                EnterPool();
            }

        }

        public bool CheckIsSelf(GameObject obj)
        {
            if(mushroom == obj)
            {
                return true;
            }
            return false;
        }

        public void ResetMushRoom()
        {
            mushRoomPool.Add(this);
            mushroom.transform.SetParent(poolTran);
            mushroom.transform.localPosition = Vector3.zero;
            mushroom.SetActive(false);
        }
    }
}