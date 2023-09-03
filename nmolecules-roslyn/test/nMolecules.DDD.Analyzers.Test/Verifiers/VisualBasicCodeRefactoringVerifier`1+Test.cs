﻿using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace NMolecules.DDD.Analyzers.Test.Verifiers;

public static partial class VisualBasicCodeRefactoringVerifier<TCodeRefactoring>
    where TCodeRefactoring : CodeRefactoringProvider, new()
{
    public class VisualBasicTest : VisualBasicCodeRefactoringTest<TCodeRefactoring, XUnitVerifier>
    {
    }
}