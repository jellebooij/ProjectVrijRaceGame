using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using Unity.Collections;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public abstract class BasePacket
{

    public abstract DataStreamWriter Write();
    public abstract void Read(DataStreamReader reader, ref DataStreamReader.Context context);
    
}
