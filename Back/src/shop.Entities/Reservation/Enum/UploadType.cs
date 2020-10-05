using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation.Enum
{
    public enum UploadType
    {
        User=0,
        Product=1,
        DocumentsProvider=2,
        Category=3,
        SubService=4,
        Organization=5,
        Field=6,
        CSV = 7,
        Order=8

    }

    public enum Dimensions
    {
        Width,
        Height
    }
}
