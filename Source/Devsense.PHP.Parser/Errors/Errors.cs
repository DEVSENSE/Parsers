// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Diagnostics;

using Devsense.PHP.Text;
using Devsense.PHP.Syntax;

namespace Devsense.PHP.Errors
{
    #region ShortPosition

    /// <summary>
    /// Position of declarations stored in tables. Used for composing error messages.
    /// </summary>
    /// <remarks>
    /// All declarations from included script have the same number.
    /// </remarks>
    public struct ShortPosition
    {
        public int Line;
        public int Column;

        /// <summary>
        /// Constructs new position.
        /// </summary>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        public ShortPosition(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Constructs new position.
        /// </summary>
        /// <param name="position">Position within document.</param>
        public ShortPosition(TextPoint position)
            :this(position.Line, position.Column)
        {
        }

        /// <summary>
        /// Returns string representation of the position - "(_line_,_column_)" or empty string for invalid position.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!IsValid)
                // empty position
                return String.Empty;

            return string.Concat('(', Line.ToString(), ',', Column.ToString(), ')');
        }

        /// <summary>
        /// Sets the position that indicates invalid positon.
        /// </summary>
        public static ShortPosition Invalid = new ShortPosition(-1, -1);

        /// <summary>
        /// Tests whether the position is valid.
        /// </summary>
        /// <returns>True if the position is valid.</returns>
        public bool IsValid
        {
            get { return Line != -1; }
        }
    }

    #endregion

    #region Warnings, Errors, Fatal Errors

    public static class Warnings
    {
        //public static readonly ErrorInfo RelatedLocation = new ErrorInfo(-1, "__related_location", ErrorSeverity.Warning);
        //public static readonly ErrorInfo None = new ErrorInfo_(-2, "", ErrorSeverity.Warning);

        //// deferred-to-runtime group:
        //public static readonly ErrorInfo InclusionReplacementFailes = new ErrorInfo_(1, "inclusion_replacement_failed", WarningGroups.InclusionsMapping);
        //public static readonly ErrorInfo InclusionTargetProcessingFailed = new ErrorInfo_(3, "incuded_file_name_processing_failed", WarningGroups.DeferredToRuntimeOthers);
        //public static readonly ErrorInfo InclusionDeferredToRuntime = new ErrorInfo_(4, "inclusion_deferred_to_runtime", WarningGroups.DeferredToRuntimeOthers);
        //// TODO: public static readonly ErrorInfo CyclicInclusionDetected = new ErrorInfo_(5, "cyclic_inclusion", WarningGroups.DeferredToRuntimeOthers);

        //// compiler-strict group:
        //public static readonly ErrorInfo ActualParamWithAmpersand = new ErrorInfo_(7, "act_param_with_ampersand", WarningGroups.AmpModifiers);
        //public static readonly ErrorInfo UnreachableCodeDetected = new ErrorInfo_(8, "unreachable_code", WarningGroups.CompilerStrictOthers);
        public static readonly ErrorInfo TooBigIntegerConversion = new ErrorInfo_(9, "too_big_int_conversion", ErrorSeverity.Information);
        //public static readonly ErrorInfo TooManyLocalVariablesInFunction = new ErrorInfo_(10, "too_many_local_variables_function", WarningGroups.CompilerStrictOthers);
        //public static readonly ErrorInfo TooManyLocalVariablesInMethod = new ErrorInfo_(11, "too_many_local_variables_method", WarningGroups.CompilerStrictOthers);
        //public static readonly ErrorInfo UnoptimizedLocalsInFunction = new ErrorInfo_(12, "unoptimized_local_variables_function", WarningGroups.CompilerStrictOthers);
        public static readonly ErrorInfo TooBigDouble = new ErrorInfo_(13, "too_big_double", ErrorSeverity.Information);

        //public static readonly ErrorInfo UnusedLabel = new ErrorInfo_(15, "unused_label", WarningGroups.CompilerStrictOthers);

        ////

        //// warnings in more groups => disabling any of these will disable the warning:
        //public static readonly ErrorInfo UnknownClassUsed = new ErrorInfo_(20, "unknown_class_used", WarningGroups.DeferredToRuntimeOthers | WarningGroups.CompilerStrictOthers);
        //public static readonly ErrorInfo UnknownClassUsedWithAlias = new ErrorInfo_(21, "unknown_class_used_with_alias", WarningGroups.DeferredToRuntimeOthers | WarningGroups.CompilerStrictOthers);
        //public static readonly ErrorInfo UnknownFunctionUsed = new ErrorInfo_(22, "unknown_function_used", WarningGroups.DeferredToRuntimeOthers | WarningGroups.CompilerStrictOthers);
        //public static readonly ErrorInfo UnknownFunctionUsedWithAlias = new ErrorInfo_(23, "unknown_function_used_with_alias", WarningGroups.DeferredToRuntimeOthers | WarningGroups.CompilerStrictOthers);
        //public static readonly ErrorInfo UnknownConstantUsed = new ErrorInfo_(24, "unknown_constant_used", WarningGroups.DeferredToRuntimeOthers | WarningGroups.CompilerStrictOthers);
        //public static readonly ErrorInfo UnknownConstantUsedWithAlias = new ErrorInfo_(25, "unknown_constant_used_with_alias", WarningGroups.DeferredToRuntimeOthers | WarningGroups.CompilerStrictOthers);

        //

        // others:
        public static readonly ErrorInfo InvalidArgumentCountForMethod = new ErrorInfo_(115, "invalid_argument_count_for_method", ErrorSeverity.Warning);
        public static readonly ErrorInfo TooFewFunctionParameters = new ErrorInfo_(116, "too_few_function_params", ErrorSeverity.Warning);
        public static readonly ErrorInfo TooFewMethodParameters = new ErrorInfo_(117, "too_few_method_params", ErrorSeverity.Warning);
        public static readonly ErrorInfo TooFewCtorParameters = new ErrorInfo_(118, "too_few_ctor_params", ErrorSeverity.Warning);
        public static readonly ErrorInfo NoCtorDefined = new ErrorInfo_(120, "no_ctor_defined", ErrorSeverity.Warning);
        public static readonly ErrorInfo MultipleSwitchCasesWithSameValue = new ErrorInfo_(121, "more_switch_cases_with_same_value", ErrorSeverity.Warning);
        public static readonly ErrorInfo MoreThenOneDefaultInSwitch = new ErrorInfo_(122, "more_then_one_default_in_switch", ErrorSeverity.Warning);
        public static readonly ErrorInfo ThisOutOfMethod = new ErrorInfo_(123, "this_out_of_method", ErrorSeverity.Warning);
        public static readonly ErrorInfo ThisInWriteContext = new ErrorInfo_(124, "this_in_write_context", ErrorSeverity.Warning);
        public static readonly ErrorInfo MandatoryBehindOptionalParam = new ErrorInfo_(125, "mandatory_behind_optional_param", ErrorSeverity.Warning);
        //public static readonly ErrorInfo InclusionReplacementFailed = new ErrorInfo_(126, "inclusion_replacement_failed", WarningGroups.InclusionsMapping);
        public static readonly ErrorInfo ConditionallyRedeclared = new ErrorInfo_(127, "conditionally_redeclared", ErrorSeverity.Warning);
        public static readonly ErrorInfo ConditionallyRedeclaredByInclusion = new ErrorInfo_(128, "conditionally_redeclared_by_inclusion", ErrorSeverity.Warning);

        public static readonly ErrorInfo PhpTrackVarsNotSupported = new ErrorInfo_(129, "php_track_vars_not_supported", ErrorSeverity.Warning);
        public static readonly ErrorInfo UnterminatedComment = new ErrorInfo_(130, "unterminated_comment", ErrorSeverity.Warning);
        public static readonly ErrorInfo InvalidEscapeSequenceLength = new ErrorInfo_(132, "invalid_escape_sequence_length", ErrorSeverity.Warning);
        public static readonly ErrorInfo InvalidLinePragma = new ErrorInfo_(133, "invalid_line_pragma", ErrorSeverity.Warning);


        //

        public static readonly ErrorInfo MultipleStatementsInAssertion = new ErrorInfo_(140, "multiple_statements_in_assertion", ErrorSeverity.Warning);

        //

        public static readonly ErrorInfo DivisionByZero = new ErrorInfo_(150, "division_by_zero", ErrorSeverity.Warning);
        public static readonly ErrorInfo NotSupportedFunctionCalled = new ErrorInfo_(151, "notsupported_function_called", ErrorSeverity.Warning);

        //

        public static readonly ErrorInfo ClassBehaviorMayBeUnexpected = new ErrorInfo_(160, "class_behavior_may_be_unexpected", ErrorSeverity.Warning);
        //public static readonly ErrorInfo IncompleteClass = new ErrorInfo_(161, "incomplete_class", WarningGroups.DeferredToRuntimeOthers);
        public static readonly ErrorInfo ImportDeprecated = new ErrorInfo_(162, "import_deprecated", ErrorSeverity.Warning);

        public static readonly ErrorInfo BodyOfDllImportedFunctionIgnored = new ErrorInfo_(170, "dll_import_body_ignored", ErrorSeverity.Warning);

        //
        public static readonly ErrorInfo MagicMethodMustBePublicNonStatic = new ErrorInfo_(171, "magic_method_must_be_public_nonstatic", ErrorSeverity.Warning);
        public static readonly ErrorInfo CallStatMustBePublicStatic = new ErrorInfo_(172, "callstat_must_be_public_static", ErrorSeverity.Warning);

        // strict standards

        public static readonly ErrorInfo DeclarationShouldBeCompatible = new ErrorInfo_(180, "declaration_should_be_compatible", ErrorSeverity.Warning);
        public static readonly ErrorInfo AssignNewByRefDeprecated = new ErrorInfo_(181, "assign_new_as_ref_is_deprecated", ErrorSeverity.Warning);

    }

    // 1000+
    public static class Errors
    {
        public static readonly ErrorInfo RelatedLocation = new ErrorInfo_(-1, "__related_location", ErrorSeverity.Error);

        public static readonly ErrorInfo ArrayInClassConstant = new ErrorInfo_(1000, "array_in_cls_const", ErrorSeverity.Error);
        public static readonly ErrorInfo NonVariablePassedByRef = new ErrorInfo_(1001, "nonvar_passed_by_ref", ErrorSeverity.Error);
        public static readonly ErrorInfo FieldInInterface = new ErrorInfo_(1002, "field_in_interface", ErrorSeverity.Error);
        public static readonly ErrorInfo InvalidBreakLevelCount = new ErrorInfo_(1003, "invalid_break_level_count", ErrorSeverity.Error);

        public static readonly ErrorInfo PropertyDeclaredAbstract = new ErrorInfo_(1004, "property_declared_abstract", ErrorSeverity.Error);
        public static readonly ErrorInfo PropertyDeclaredFinal = new ErrorInfo_(1005, "property_declared_final", ErrorSeverity.Error);
        public static readonly ErrorInfo PropertyRedeclared = new ErrorInfo_(1006, "property_redeclared", ErrorSeverity.Error);

        public static readonly ErrorInfo MethodRedeclared = new ErrorInfo_(1007, "method_redeclared", ErrorSeverity.Error);
        public static readonly ErrorInfo InterfaceMethodWithBody = new ErrorInfo_(1008, "interface_bodyful_method", ErrorSeverity.Error);
        public static readonly ErrorInfo AbstractMethodWithBody = new ErrorInfo_(1009, "abstract_bodyful_method", ErrorSeverity.Error);
        public static readonly ErrorInfo NonAbstractMethodWithoutBody = new ErrorInfo_(1010, "nonabstract_bodyless_method", ErrorSeverity.Error);
        public static readonly ErrorInfo CloneCannotTakeArguments = new ErrorInfo_(1011, "clone_cannot_take_arguments", ErrorSeverity.Error);
        public static readonly ErrorInfo CloneCannotBeStatic = new ErrorInfo_(1012, "clone_cannot_be_static", ErrorSeverity.Error);
        public static readonly ErrorInfo DestructCannotTakeArguments = new ErrorInfo_(1013, "destruct_cannot_take_arguments", ErrorSeverity.Error);
        public static readonly ErrorInfo DestructCannotBeStatic = new ErrorInfo_(1014, "destruct_cannot_be_static", ErrorSeverity.Error);
        public static readonly ErrorInfo AbstractPrivateMethodDeclared = new ErrorInfo_(1015, "abstract_private_method_declared", ErrorSeverity.Error);
        public static readonly ErrorInfo InterfaceMethodNotPublic = new ErrorInfo_(1016, "interface_method_non_public", ErrorSeverity.Error);
        public static readonly ErrorInfo ConstantRedeclared = new ErrorInfo_(1017, "constant_redeclared", ErrorSeverity.Error);
        public static readonly ErrorInfo AbstractMethodNotImplemented = new ErrorInfo_(1018, "abstract_method_not_implemented", ErrorSeverity.Error);
        public static readonly ErrorInfo MethodNotCompatible = new ErrorInfo_(1019, "method_not_compatible", ErrorSeverity.Error);   //  TODO: it's fatal error

        public static readonly ErrorInfo NonInterfaceImplemented = new ErrorInfo_(1020, "non_interface_implemented", ErrorSeverity.Error);
        public static readonly ErrorInfo NonInterfaceExtended = new ErrorInfo_(1021, "non_interface_extended", ErrorSeverity.Error);
        public static readonly ErrorInfo NonClassExtended = new ErrorInfo_(1022, "non_class_extended", ErrorSeverity.Error);

        public static readonly ErrorInfo FinalClassExtended = new ErrorInfo_(1023, "final_class_extended", ErrorSeverity.Error);

        // 1023

        public static readonly ErrorInfo ConstructCannotBeStatic = new ErrorInfo_(1024, "construct_cannot_be_static", ErrorSeverity.Error);
        public static readonly ErrorInfo OverrideFinalMethod = new ErrorInfo_(1025, "override_final_method", ErrorSeverity.Error);
        public static readonly ErrorInfo MakeStaticMethodNonStatic = new ErrorInfo_(1026, "make_static_method_non_static", ErrorSeverity.Error);
        public static readonly ErrorInfo MakeNonStaticMethodStatic = new ErrorInfo_(1027, "make_nonstatic_method_static", ErrorSeverity.Error);
        public static readonly ErrorInfo OverridingNonAbstractMethodByAbstract = new ErrorInfo_(1028, "nonabstract_method_overridden_with_abstract", ErrorSeverity.Error);
        public static readonly ErrorInfo OverridingMethodRestrictsVisibility = new ErrorInfo_(1029, "overriding_method_restrict_visibility", ErrorSeverity.Error);
        public static readonly ErrorInfo MakeStaticPropertyNonStatic = new ErrorInfo_(1030, "make_static_property_nonstatic", ErrorSeverity.Error);
        public static readonly ErrorInfo MakeNonStaticPropertyStatic = new ErrorInfo_(1031, "make_nonstatic_property_static", ErrorSeverity.Error);
        public static readonly ErrorInfo OverridingFieldRestrictsVisibility = new ErrorInfo_(1032, "overriding_property_restrict_visibility", ErrorSeverity.Error);
        public static readonly ErrorInfo OverridingStaticFieldByStatic = new ErrorInfo_(1033, "overriding_static_field_with_static", ErrorSeverity.Error);
        public static readonly ErrorInfo OverridingProtectedStaticWithInitValue = new ErrorInfo_(1034, "overriding_protected_static_with_init_value", ErrorSeverity.Error);
        public static readonly ErrorInfo InheritingOnceInheritedConstant = new ErrorInfo_(1035, "inheriting_previously_inherited_constant", ErrorSeverity.Error);
        public static readonly ErrorInfo RedeclaringInheritedConstant = new ErrorInfo_(1036, "redeclaring_inherited_constant", ErrorSeverity.Error);
        public static readonly ErrorInfo AbstractFinalMethodDeclared = new ErrorInfo_(1037, "abstract_final_method_declared", ErrorSeverity.Error);
        public static readonly ErrorInfo LibraryFunctionRedeclared = new ErrorInfo_(1038, "library_func_redeclared", ErrorSeverity.Error);
        public static readonly ErrorInfo DuplicateParameterName = new ErrorInfo_(1039, "duplicate_parameter_name", ErrorSeverity.Error);
        public static readonly ErrorInfo EmptyIndexInReadContext = new ErrorInfo_(1040, "empty_index_in_read_context", ErrorSeverity.Error);


        public static readonly ErrorInfo ConstructNotSupported = new ErrorInfo_(1041, "construct_not_supported", ErrorSeverity.Error);
        public static readonly ErrorInfo KeyAlias = new ErrorInfo_(1042, "key_alias", ErrorSeverity.Error);
        public static readonly ErrorInfo MultipleVisibilityModifiers = new ErrorInfo_(1043, "multiple_visibility_modifiers", ErrorSeverity.Error);
        public static readonly ErrorInfo InvalidInterfaceModifier = new ErrorInfo_(1044, "invalid_interface_modifier", ErrorSeverity.Error);

        public static readonly ErrorInfo MethodCannotTakeArguments = new ErrorInfo_(1045, "method_cannot_take_arguments", ErrorSeverity.Error);

        //

        public static readonly ErrorInfo PrivateClassInGlobalNamespace = new ErrorInfo_(1048, "private_class_in_global_ns", ErrorSeverity.Error);


        public static readonly ErrorInfo InvalidCodePoint = new ErrorInfo_(1049, "invalid_code_point", ErrorSeverity.Error);
        public static readonly ErrorInfo InvalidCodePointName = new ErrorInfo_(1050, "invalid_code_point_name", ErrorSeverity.Error);
        public static readonly ErrorInfo InclusionInPureUnit = new ErrorInfo_(1051, "inclusion_in_pure_unit", ErrorSeverity.Error);
        public static readonly ErrorInfo GlobalCodeInPureUnit = new ErrorInfo_(1052, "global_code_in_pure_unit", ErrorSeverity.Error);

        public static readonly ErrorInfo ConflictingTypeAliases = new ErrorInfo_(1053, "conflicting_type_aliases", ErrorSeverity.Error);
        public static readonly ErrorInfo ConflictingFunctionAliases = new ErrorInfo_(1054, "conflicting_function_aliases", ErrorSeverity.Error);
        public static readonly ErrorInfo ConflictingConstantAliases = new ErrorInfo_(1055, "conflicting_constant_aliases", ErrorSeverity.Error);

        //

        public static readonly ErrorInfo ProtectedPropertyAccessed = new ErrorInfo_(1058, "protected_property_accessed", ErrorSeverity.Error);
        public static readonly ErrorInfo PrivatePropertyAccessed = new ErrorInfo_(1059, "private_property_accessed", ErrorSeverity.Error);
        public static readonly ErrorInfo ProtectedMethodCalled = new ErrorInfo_(1060, "protected_method_called", ErrorSeverity.Error);
        public static readonly ErrorInfo PrivateMethodCalled = new ErrorInfo_(1061, "private_method_called", ErrorSeverity.Error);
        public static readonly ErrorInfo PrivateCtorCalled = new ErrorInfo_(1062, "private_ctor_called", ErrorSeverity.Error);
        public static readonly ErrorInfo ProtectedCtorCalled = new ErrorInfo_(1063, "protected_ctor_called", ErrorSeverity.Error);
        public static readonly ErrorInfo ProtectedConstantAccessed = new ErrorInfo_(1064, "protected_constant_accessed", ErrorSeverity.Error);
        public static readonly ErrorInfo PrivateConstantAccessed = new ErrorInfo_(1065, "private_constant_accessed", ErrorSeverity.Error);

        public static readonly ErrorInfo UnknownMethodCalled = new ErrorInfo_(1066, "unknown_method_called", ErrorSeverity.Error);
        public static readonly ErrorInfo AbstractMethodCalled = new ErrorInfo_(1067, "abstract_method_called", ErrorSeverity.Error);
        public static readonly ErrorInfo UnknownPropertyAccessed = new ErrorInfo_(1068, "undeclared_static_property_accessed", ErrorSeverity.Error);
        public static readonly ErrorInfo UnknownClassConstantAccessed = new ErrorInfo_(1069, "undefined_class_constant", ErrorSeverity.Error);
        public static readonly ErrorInfo CircularConstantDefinitionGlobal = new ErrorInfo_(1070, "circular_constant_definition_global", ErrorSeverity.Error);
        public static readonly ErrorInfo CircularConstantDefinitionClass = new ErrorInfo_(1071, "circular_constant_definition_class", ErrorSeverity.Error);

        //

        public static readonly ErrorInfo MissingEntryPoint = new ErrorInfo_(1075, "missing_entry_point", ErrorSeverity.Error);
        public static readonly ErrorInfo EntryPointRedefined = new ErrorInfo_(1076, "entry_point_redefined", ErrorSeverity.Error);


        //

        public static readonly ErrorInfo AmbiguousTypeMatch = new ErrorInfo_(1088, "ambiguous_type_match", ErrorSeverity.Error);
        public static readonly ErrorInfo AmbiguousFunctionMatch = new ErrorInfo_(1089, "ambiguous_function_match", ErrorSeverity.Error);
        public static readonly ErrorInfo AmbiguousConstantMatch = new ErrorInfo_(1090, "ambiguous_constant_match", ErrorSeverity.Error);
        public static readonly ErrorInfo CannotUseReservedName = new ErrorInfo_(1091, "cannot_use_reserved_name", ErrorSeverity.Error);
        public static readonly ErrorInfo IncompleteClass = new ErrorInfo_(1092, "incomplete_class", ErrorSeverity.Error);
        public static readonly ErrorInfo ClassHasNoVisibleCtor = new ErrorInfo_(1093, "class_has_no_visible_ctor", ErrorSeverity.Error);
        public static readonly ErrorInfo AbstractClassOrInterfaceInstantiated = new ErrorInfo_(1094, "abstract_class_or_interface_instantiated", ErrorSeverity.Error);
        public static readonly ErrorInfo ClosureInstantiated = new ErrorInfo_(1095, "instantiation_not_allowed", ErrorSeverity.Error);

        //

        public static readonly ErrorInfo ParentUsedOutOfClass = new ErrorInfo_(1110, "parent_used_out_of_class", ErrorSeverity.Error);
        public static readonly ErrorInfo SelfUsedOutOfClass = new ErrorInfo_(1111, "self_used_out_of_class", ErrorSeverity.Error);
        public static readonly ErrorInfo ClassHasNoParent = new ErrorInfo_(1112, "class_has_no_parent", ErrorSeverity.Error);
        public static readonly ErrorInfo StaticUsedOutOfClass = new ErrorInfo_(1113, "static_used_out_of_class", ErrorSeverity.Error);

        //

        public static readonly ErrorInfo UnknownCustomAttribute = new ErrorInfo_(1120, "unknown_custom_attribute", ErrorSeverity.Error);
        public static readonly ErrorInfo NotCustomAttributeClass = new ErrorInfo_(1121, "not_custom_attribute_class", ErrorSeverity.Error);
        public static readonly ErrorInfo InvalidAttributeExpression = new ErrorInfo_(1122, "invalid_attribute_expression", ErrorSeverity.Error);
        public static readonly ErrorInfo InvalidAttributeUsage = new ErrorInfo_(1123, "invalid_attribute_usage", ErrorSeverity.Error);
        public static readonly ErrorInfo InvalidAttributeTargetSelector = new ErrorInfo_(1124, "invalid_attribute_target_selector", ErrorSeverity.Error);
        public static readonly ErrorInfo DuplicateAttributeUsage = new ErrorInfo_(1125, "duplicate_attribute_usage", ErrorSeverity.Error);
        public static readonly ErrorInfo OutAttributeOnByValueParam = new ErrorInfo_(1126, "out_attribute_on_byval_param", ErrorSeverity.Error);
        public static readonly ErrorInfo ExportAttributeInNonPureUnit = new ErrorInfo_(1127, "export_attribute_in_non_pure", ErrorSeverity.Error);


        //

        public static readonly ErrorInfo MissingPartialModifier = new ErrorInfo_(1148, "missing_partial_modifier", ErrorSeverity.Error);
        public static readonly ErrorInfo PartialConditionalDeclaration = new ErrorInfo_(1149, "partial_conditional_declaration", ErrorSeverity.Error);
        public static readonly ErrorInfo PartialTransientDeclaration = new ErrorInfo_(1150, "partial_transient_declaration", ErrorSeverity.Error);
        public static readonly ErrorInfo PartialImpureDeclaration = new ErrorInfo_(1151, "partial_impure_declaration", ErrorSeverity.Error);
        public static readonly ErrorInfo IncompatiblePartialDeclarations = new ErrorInfo_(1152, "incompatible_partial_declarations", ErrorSeverity.Error);
        public static readonly ErrorInfo ConflictingPartialVisibility = new ErrorInfo_(1153, "conflicting_partial_visibility", ErrorSeverity.Error);
        public static readonly ErrorInfo PartialDeclarationsDifferInBase = new ErrorInfo_(1154, "partial_declarations_differ_in_base", ErrorSeverity.Error);
        public static readonly ErrorInfo PartialDeclarationsDifferInTypeParameterCount = new ErrorInfo_(1155, "partial_declarations_differ_in_type_parameter_count", ErrorSeverity.Error);
        public static readonly ErrorInfo PartialDeclarationsDifferInTypeParameter = new ErrorInfo_(1156, "partial_declarations_differ_in_type_parameter", ErrorSeverity.Error);

        //

        public static readonly ErrorInfo GenericParameterMustBeType = new ErrorInfo_(1170, "generic_parameter_must_be_type", ErrorSeverity.Error);
        public static readonly ErrorInfo DuplicateGenericParameter = new ErrorInfo_(1171, "duplicate_generic_parameter", ErrorSeverity.Error);
        public static readonly ErrorInfo GenericParameterCollidesWithDeclarer = new ErrorInfo_(1172, "generic_parameter_collides_with_declarer", ErrorSeverity.Error);
        public static readonly ErrorInfo CannotDeriveFromTypeParameter = new ErrorInfo_(1173, "cannot_derive_from_type_parameter", ErrorSeverity.Error);
        public static readonly ErrorInfo GenericCallToLibraryFunction = new ErrorInfo_(1074, "generic_call_to_library_function", ErrorSeverity.Error);
        public static readonly ErrorInfo ConstructorWithGenericParameters = new ErrorInfo_(1075, "generic_parameters_disallowed_on_ctor", ErrorSeverity.Error);
        public static readonly ErrorInfo GenericAlreadyInUse = new ErrorInfo_(1076, "generic_in_use", ErrorSeverity.Error);

        public static readonly ErrorInfo TooManyTypeArgumentsInTypeUse = new ErrorInfo_(1080, "too_many_type_arguments_in_type_use", ErrorSeverity.Error);
        public static readonly ErrorInfo NonGenericTypeUsedWithTypeArgs = new ErrorInfo_(1081, "non_generic_type_used_with_type_arguments", ErrorSeverity.Error);
        public static readonly ErrorInfo MissingTypeArgumentInTypeUse = new ErrorInfo_(1082, "missing_type_argument_in_type_use", ErrorSeverity.Error);
        public static readonly ErrorInfo IncompatibleTypeParameterConstraintsInTypeUse = new ErrorInfo_(1083, "incompatible_type_parameter_constraints_type", ErrorSeverity.Error);
        public static readonly ErrorInfo IncompatibleTypeParameterConstraintsInMethodUse = new ErrorInfo_(1084, "incompatible_type_parameter_constraints_method", ErrorSeverity.Error);

        //

        public static readonly ErrorInfo InvalidArgumentCount = new ErrorInfo_(1210, "invalid_argument_count", ErrorSeverity.Error);
        public static readonly ErrorInfo AbstractPropertyNotImplemented = new ErrorInfo_(1211, "abstract_property_not_implemented", ErrorSeverity.Error);
        public static readonly ErrorInfo InvalidArgumentCountForFunction = new ErrorInfo_(1212, "invalid_argument_count_for_function", ErrorSeverity.Error);


        //

        public static readonly ErrorInfo InvalidIdentifier = new ErrorInfo_(1230, "invalid_identifier", ErrorSeverity.Error);

        public static readonly ErrorInfo LabelRedeclared = new ErrorInfo_(1231, "label_redeclared", ErrorSeverity.Error);
        public static readonly ErrorInfo UndefinedLabel = new ErrorInfo_(1232, "undefined_label", ErrorSeverity.Error);

        //

        public static readonly ErrorInfo ExpectingParentCtorInvocation = new ErrorInfo_(1240, "expecting_parent_ctor_invocation", ErrorSeverity.Error);
        public static readonly ErrorInfo UnexpectedParentCtorInvocation = new ErrorInfo_(1241, "unexpected_parent_ctor_invocation", ErrorSeverity.Error);
        public static readonly ErrorInfo MissingCtorInClrSubclass = new ErrorInfo_(1242, "missing_ctor_in_clr_subclass", ErrorSeverity.Error);

        public static readonly ErrorInfo MissingImportedEntity = new ErrorInfo_(1250, "missing_imported_entity", ErrorSeverity.Error);
        public static readonly ErrorInfo NamespaceKeywordUsedOutsideOfNamespace = new ErrorInfo_(1251, "namespace_keyword_outside_namespace", ErrorSeverity.Error);
        public static readonly ErrorInfo ImportOnlyInPureMode = new ErrorInfo_(1252, "import_only_in_pure", ErrorSeverity.Error);

        public static readonly ErrorInfo DllImportMethodMustBeStatic = new ErrorInfo_(1260, "dll_import_must_be_static", ErrorSeverity.Error);
        public static readonly ErrorInfo DllImportMethodCannotBeAbstract = new ErrorInfo_(1261, "dll_import_cannot_be_abstract", ErrorSeverity.Error);
    }

    // 2000+
    public static class FatalErrors
    {
        public static readonly ErrorInfo RelatedLocation = new ErrorInfo_(-1, "__related_location", ErrorSeverity.Error);

        public static readonly ErrorInfo TypeRedeclared = new ErrorInfo_(2000, "type_redeclared", ErrorSeverity.FatalError);
        public static readonly ErrorInfo FunctionRedeclared = new ErrorInfo_(2001, "function_redeclared", ErrorSeverity.FatalError);
        public static readonly ErrorInfo ConstantRedeclared = new ErrorInfo_(2002, "constant_redeclared", ErrorSeverity.FatalError);

        public static readonly ErrorInfo InvalidCommandLineArgument = new ErrorInfo_(2003, "invalid_command_line_argument", ErrorSeverity.FatalError);
        public static readonly ErrorInfo InvalidCommandLineArgumentNoName = new ErrorInfo_(2003, "invalid_command_line_argument_noname", ErrorSeverity.FatalError);
        public static readonly ErrorInfo ConfigurationError = new ErrorInfo_(2004, "configuration_error", ErrorSeverity.FatalError);
        public static readonly ErrorInfo InvalidSource = new ErrorInfo_(2005, "invalid_source", ErrorSeverity.FatalError);
        public static readonly ErrorInfo ErrorCreatingFile = new ErrorInfo_(2006, "error_creating_file", ErrorSeverity.FatalError);
        public static readonly ErrorInfo InternalError = new ErrorInfo_(2007, "internal_error", ErrorSeverity.FatalError);

        public static readonly ErrorInfo InvalidHaltCompiler = new ErrorInfo_(2008, "invalid_halt_compiler", ErrorSeverity.FatalError);


        //public static readonly ErrorInfo RedeclaredByInclusion = new ErrorInfo_(2005, "redeclared_by_inclusion", ErrorSeverity.FatalError);



        //public static readonly ErrorInfo ClassRedeclaredAtRuntime = new ErrorInfo_(2007, "class_redeclared_runtime", ErrorSeverity.FatalError);
        //public static readonly ErrorInfo ClassRedeclaredAtRuntimeByInclusion = new ErrorInfo_(2008, "class_redeclared_runtime_include", ErrorSeverity.FatalError);



        //public static readonly ErrorInfo LibraryClassRedeclared = new ErrorInfo_(2009, "library_class_redeclared", ErrorSeverity.FatalError);
        //public static readonly ErrorInfo LibraryClassRedeclaredByInclusion = new ErrorInfo_(2010, "library_class_redeclared_by_inclusion", ErrorSeverity.FatalError);
        //public static readonly ErrorInfo ClassRedeclaredInAssembly = new ErrorInfo_(2011, "class_redeclared_in_assembly", ErrorSeverity.FatalError);
        //public static readonly ErrorInfo AbstractMethodNameNotMatchingImplementation = new ErrorInfo_(2012, "abstract_method_name_not_matching_implementation", ErrorSeverity.FatalError);
        public static readonly ErrorInfo SyntaxError = new SyntaxError();
        public static readonly ErrorInfo CheckVarUseFault = new ErrorInfo_(2015, "check_varuse_fault", ErrorSeverity.FatalError);

        public static readonly ErrorInfo CircularBaseClassDependency = new ErrorInfo_(2030, "circular_base_class_dependency", ErrorSeverity.FatalError);
        public static readonly ErrorInfo CircularBaseInterfaceDependency = new ErrorInfo_(2031, "circular_base_interface_dependency", ErrorSeverity.FatalError);
        public static readonly ErrorInfo MethodMustTakeExacArgsCount = new ErrorInfo_(2032, "method_must_take_exact_args_count", ErrorSeverity.FatalError);

        public static readonly ErrorInfo AliasAlreadyInUse = new ErrorInfo_(2040, "alias_in_use", ErrorSeverity.FatalError);
        public static readonly ErrorInfo ClassAlreadyInUse = new ErrorInfo_(2041, "class_in_use", ErrorSeverity.FatalError);

        public static readonly ErrorInfo TryWithoutCatchOrFinally = new ErrorInfo_(2050, "try_without_catch_or_finally", ErrorSeverity.FatalError);

        public static readonly ErrorInfo MixedNamespacedeclarations = new ErrorInfo_(2060, "mixed_namespace_declaration", ErrorSeverity.FatalError);

        public static readonly ErrorInfo ParentAccessedInParentlessClass = new ErrorInfo_(2070, "parent_accessed_in_parentless_class", ErrorSeverity.FatalError);
    }

    #endregion
}
