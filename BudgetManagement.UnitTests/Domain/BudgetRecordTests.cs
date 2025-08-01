using BudgetManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.UnitTests.Domain;

public class BudgetRecordTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenCodeIsNull()
    {
        Assert.Throws<ArgumentException>(() => new BudgetRecord(""));
    }

    [Fact]
    public void Constructor_ShouldSetCode_WhenValid()
    {
        var code = "PRJ-001";
        var record = new BudgetRecord(code);
        Assert.Equal(code, record.SubProjectCode);
    }
}
