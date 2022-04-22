using System.Collections.Generic;
using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels;

public record SeasonMeets(Season Season, List<Meet> IndoorMeets, List<Meet> OutdoorMeets);