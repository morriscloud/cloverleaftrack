using System;

namespace CloverleafTrack.Models;

[Flags]
public enum GraduationStatus
{
    None = 0,
    Graduated = 1,
    InSchool = 2
}