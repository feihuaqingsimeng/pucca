using Common.Packet;
using System;
using System.Collections.Generic;

public class Data : Node
{
    public string PersonalInformationUsageAgreement
    {
        get;
        private set;
    }

    public string TermsOfService
    {
        get;
        private set;
    }

    public delegate void OnTerms(string personalInformationUsageAgreement, string termsOfService);
    public OnTerms onTerms;

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_CONST_TABLE_LIST_ACK>(RCV_PACKET_CG_READ_CONST_TABLE_LIST_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_AUTH_GET_TERMS_ACK>(RCV_PACKET_CG_AUTH_GET_TERMS_ACK);

        return base.OnCreate();
    }

    public void REQ_PACKET_CG_READ_CONST_TABLE_LIST_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_CONST_TABLE_LIST_SYN());
    }

    public void REQ_PACKET_CG_AUTH_GET_TERMS_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_AUTH_GET_TERMS_SYN());
    }

    void RCV_PACKET_CG_READ_CONST_TABLE_LIST_ACK(PACKET_CG_READ_CONST_TABLE_LIST_ACK packet)
    {
        if (packet.m_ConstTable != null
            && packet.m_ConstTable.Count > 0)
        {
            if (DB_Const.instance.schemaList == null)
            {
                //
            }
            else if (DB_Const.instance.schemaList != null
                     && DB_Const.instance.schemaList.Count > 0)
            {
                DB_Const.instance.schemaList.Clear();
            }

            for (int i = 0; i < packet.m_ConstTable.Count; i++)
            {
                CConstTableData constTableData = packet.m_ConstTable[i];
                if (constTableData != null)
                {
                    if (!string.IsNullOrEmpty(constTableData.Const_IndexID))
                    {
                        Const_IndexID constIndexID = 0;
                        bool isParseFailed = false;
                        // ArgumentNullException or ArgumentException.
                        try
                        {
                            constIndexID = (Const_IndexID)Enum.Parse(typeof(Const_IndexID), constTableData.Const_IndexID, true);
                        }
                        catch (Exception exception)
                        {
                            isParseFailed = true;
                            LogError(exception.ToString());
                        }

                        if (!isParseFailed)
                        {
                            DB_Const.instance.schemaList.Add(new DB_Const.Schema()
                            {
                                Index = constTableData.Index,
                                Const_IndexID = constIndexID,
                                Const_Value = constTableData.Const_Value,
                            });
                        }
                    }
                    else LogError("CConstTableData.Const_IndexID is null or empty.");
                }
                else LogError("CConstTableData is null.");
            }

            DB_Const.instance.GenerateCache(DB_Const.Field.Const_IndexID);
        }
        else LogError("PACKET_CG_READ_CONST_TABLE_LIST_ACK.m_ConstTable is null or empty.");
    }

    void RCV_PACKET_CG_AUTH_GET_TERMS_ACK(PACKET_CG_AUTH_GET_TERMS_ACK packet)
    {
        // var TermOfMseed = packet.m_sMSeedTerms;
        PersonalInformationUsageAgreement = packet.m_sPersonalInfoTerms;
        TermsOfService = packet.m_sServiceTerms;

        if (onTerms != null)
        {
            onTerms(PersonalInformationUsageAgreement, TermsOfService);
        }
    }

    public T GetValue<T>(Const_IndexID constIndexID) where T : IFormattable
    {
        DB_Const.Schema table = DB_Const.Query(DB_Const.Field.Const_IndexID, constIndexID);
        if (table != null)
        {
            return (T)Convert.ChangeType(table.Const_Value, typeof(T));
        }

        return default(T);
    }
}
