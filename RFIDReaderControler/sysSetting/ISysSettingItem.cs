using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RFIDReaderControler
{
    public interface ISysSettingItem
    {
        //添加和清除控件
        void addControls();
        void removeControls();

        //设置更改
        bool isChanged();
        bool saveChanges();

    }
}
