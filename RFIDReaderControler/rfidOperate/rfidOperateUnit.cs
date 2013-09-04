using System;
using System.Collections.Generic;
using System.Text;

namespace RfidReader
{
    public class rfidOperateUnit2600InventoryTag : rfidOperateUnitBase
    {
        public rfidOperateUnit2600InventoryTag(IDataTransfer _dataTransfer)
            : base(_dataTransfer,enumRFIDType.RFID2600)
        {
            reader2600OperateAction op = new reader2600OperateAction();
            //op.bLoop = true;
            this.actionList.Add(op);
        }

    }
    public class rfidOperateUnitStopInventoryTag : rfidOperateUnitBase
    {
        public rfidOperateUnitStopInventoryTag(IDataTransfer _dataTransfer)
            : base(_dataTransfer,enumRFIDType.RMU900)
        {
            operateActionRmu900StopGet op = new operateActionRmu900StopGet();

            this.actionList.Add(op);
        }
    }
    public class rfidOperateUnitInventoryTag : rfidOperateUnitBase
    {
        public rfidOperateUnitInventoryTag(IDataTransfer _dataTransfer)
            : base(_dataTransfer, enumRFIDType.RMU900)
        {
            this.actionList.Add(new operateActionRmu900CheckReady());
            this.actionList.Add(new operateActionRmu900InventoryAnti());
            operateActionRmu900InventoryAntiNoStopGet op = new operateActionRmu900InventoryAntiNoStopGet();
            op.bLoop = true;
            this.actionList.Add(op);

        }
    }
    public class rfidOperateUnitWirteEPC : rfidOperateUnitBase
    {
        public void setEPC(string epc)
        {
            operateActionRmu900InventoryWriteEpc op = (operateActionRmu900InventoryWriteEpc)this.actionList[1];
            op.setEPC(epc);
        }
        public rfidOperateUnitWirteEPC(IDataTransfer _dataTransfer)
            : base(_dataTransfer, enumRFIDType.RMU900)
        {
            this.actionList.Add(new operateActionRmu900CheckReady());
            this.actionList.Add(new operateActionRmu900InventoryWriteEpc());
        }
        public rfidOperateUnitWirteEPC(string epc, IDataTransfer _dataTransfer)
            : base(_dataTransfer, enumRFIDType.RMU900)
        {
            this.actionList.Add(new operateActionRmu900CheckReady());
            this.actionList.Add(new operateActionRmu900InventoryWriteEpc(epc));
        }
    }


    public class rfidOperateUnitGetTagEPC : rfidOperateUnitBase
    {

        public rfidOperateUnitGetTagEPC(IDataTransfer _dataTransfer)
            : base(_dataTransfer, enumRFIDType.RMU900)
        {
            this.actionList.Add(new operateActionRmu900CheckReady());
            this.actionList.Add(new operateActionRmu900InventoryAnti());
            this.actionList.Add(new operateActionRmu900InventoryAntiNoNextCommand());
            this.actionList.Add(new operateActionRmu900StopGet());
        }
    }
}
