﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Utils;
using Avalonia.UnitTests;
using Xunit;
using CollectionChangedEventManager = Avalonia.Controls.Utils.CollectionChangedEventManager;

#nullable enable

namespace Avalonia.Controls.UnitTests.Selection
{
    public class SelectionModelTests_Single
    {
        public class Source : ScopedTestBase
        {
            [Fact]
            public void Can_Select_Index_Before_Source_Assigned()
            {
                var target = CreateTarget(false);
                var raised = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Empty(e.DeselectedItems);
                    Assert.Equal(new[] { 5 }, e.SelectedIndexes);
                    Assert.Equal(new string?[] { null }, e.SelectedItems);
                    ++raised;
                };

                target.SelectedIndex = 5;

                Assert.Equal(5, target.SelectedIndex);
                Assert.Equal(new[] { 5 }, target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Equal(new string?[] { null }, target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Can_Select_Item_Before_Source_Assigned()
            {
                var target = CreateTarget(false);
                var raised = 0;

                target.SelectionChanged += (s, e) => ++raised;
                target.SelectedItem = "bar";

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new string?[] { "bar" }, target.SelectedItems);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Initializing_Source_Retains_Valid_Index_Selection()
            {
                var target = CreateTarget(false);
                var raised = 0;

                target.SelectedIndex = 1;

                target.SelectionChanged += (s, e) => ++raised;

                target.Source = new[] { "foo", "bar", "baz" };

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Initializing_Source_Removes_Invalid_Index_Selection()
            {
                var target = CreateTarget(false);
                var raised = 0;

                target.SelectedIndex = 5;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 5 }, e.DeselectedIndexes);
                    Assert.Equal(new string?[] { null }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.Source = new[] { "foo", "bar", "baz" };

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Initializing_Source_Retains_Valid_Item_Selection()
            {
                var target = CreateTarget(false);
                var raised = 0;

                target.SelectedItem = "bar";

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Empty(e.DeselectedItems);
                    Assert.Equal(new[] { 1 }, e.SelectedIndexes);
                    Assert.Equal(new string[] { "bar" }, e.SelectedItems);
                    ++raised;
                };

                target.Source = new[] { "foo", "bar", "baz" };

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Initializing_Source_Removes_Invalid_Item_Selection()
            {
                var target = CreateTarget(false);
                var raised = 0;

                target.SelectedItem = "qux";
                target.SelectionChanged += (s, e) => ++raised;
                target.Source = new[] { "foo", "bar", "baz" };

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Initializing_Source_Respects_SourceIndex_SourceItem_Order()
            {
                var target = CreateTarget(false);

                target.SelectedIndex = 0;
                target.SelectedItem = "bar";

                target.Source = new[] { "foo", "bar", "baz" };

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
            }

            [Fact]
            public void Initializing_Source_Respects_SourceItem_SourceIndex_Order()
            {
                var target = CreateTarget(false);

                target.SelectedItem = "foo";
                target.SelectedIndex = 1;

                target.Source = new[] { "foo", "bar", "baz" };

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
            }

            [Fact]
            public void Initializing_Source_Raises_SelectedItems_PropertyChanged()
            {
                var target = CreateTarget(false);
                var selectedItemRaised = 0;
                var selectedItemsRaised = 0;

                target.Select(1);

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedItem))
                    {
                        ++selectedItemRaised;
                    }
                    else if (e.PropertyName == nameof(target.SelectedItems))
                    {
                        ++selectedItemsRaised;
                    }
                };

                target.Source = new[] { "foo", "bar", "baz" };

                Assert.Equal(1, selectedItemRaised);
                Assert.Equal(1, selectedItemsRaised);
            }

            [Fact]
            public void Changing_Source_To_Null_Doesnt_Clear_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 2;

                target.SelectionChanged += (s, e) => ++raised;

                target.Source = null;

                Assert.Equal(2, target.SelectedIndex);
                Assert.Equal(new[] { 2 }, target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Equal(new string?[] { null }, target.SelectedItems);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Changing_Source_To_NonNull_First_Clears_Old_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 2;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 2 }, e.DeselectedIndexes);
                    Assert.Equal(new string?[] { "baz" }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.Source = new[] { "qux", "quux", "corge" };

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Changing_Source_To_Null_Raises_SelectedItems_PropertyChanged()
            {
                var target = CreateTarget();
                var selectedItemRaised = 0;
                var selectedItemsRaised = 0;

                target.Select(1);

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedItem))
                    {
                        ++selectedItemRaised;
                    }
                    else if (e.PropertyName == nameof(target.SelectedItems))
                    {
                        ++selectedItemsRaised;
                    }
                };

                target.Source = null;

                Assert.Equal(1, selectedItemRaised);
                Assert.Equal(1, selectedItemsRaised);
            }

            [Fact]
            public void Raises_PropertyChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.Source))
                    {
                        ++raised;
                    }
                };

                target.Source = new[] { "qux", "quux", "corge" };

                Assert.Equal(1, raised);
            }

            [Fact]
            public void Can_Assign_ValueType_Collection_To_SelectionModel_Of_Object()
            {
                var target = (ISelectionModel)new SelectionModel<object>();

                target.Source = new[] { 1, 2, 3 };
            }

            [Fact]
            public void Can_Change_Source_In_SelectedItem_Change_Handler()
            {
                // Issue #11617
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedItem) && raised == 0)
                    {
                        ++raised;
                        target.Source = new[] { "foo", "baz", "bar" };
                    }
                };

                target.SelectedIndex = 1;

                Assert.Equal(-1, target.SelectedIndex);
            }
        }

        public class SelectedIndex : ScopedTestBase
        {
            [Fact]
            public void SelectedIndex_Larger_Than_Source_Clears_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 1;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 1 }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.SelectedIndex = 5;

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Negative_SelectedIndex_Is_Coerced_To_Minus_1()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectionChanged += (s, e) => ++raised;

                target.SelectedIndex = -5;

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Setting_SelectedIndex_Clears_Old_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 0 }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "foo" }, e.DeselectedItems);
                    Assert.Equal(new[] { 1 }, e.SelectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.SelectedItems);
                    ++raised;
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Setting_SelectedIndex_During_CollectionChanged_Results_In_Correct_Selection()
            {
                // Issue #4496
                var data = new AvaloniaList<string>();
                var target = CreateTarget();
                var binding = new MockBinding(target, data);

                target.Source = data;

                data.Add("foo");

                Assert.Equal(0, target.SelectedIndex);
            }

            [Fact]
            public void PropertyChanged_Is_Raised()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndex))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, raised);
            }

            private class MockBinding : ICollectionChangedListener
            {
                private readonly SelectionModel<string?> _target;

                public MockBinding(SelectionModel<string?> target, AvaloniaList<string> data)
                {
                    _target = target;
                    CollectionChangedEventManager.Instance.AddListener(data, this);
                }

                public void Changed(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
                {
                    _target.Select(0);
                }

                public void PostChanged(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
                {
                }

                public void PreChanged(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
                {
                }
            }
        }

        public class SelectedItem : ScopedTestBase
        {
            [Fact]
            public void Setting_SelectedItem_To_Valid_Item_Updates_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Empty(e.DeselectedItems);
                    Assert.Equal(new[] { 1 }, e.SelectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.SelectedItems);
                    ++raised;
                };

                target.SelectedItem = "bar";

                Assert.Equal(1, raised);
            }

            [Fact]
            public void PropertyChanged_Is_Raised_When_SelectedIndex_Changes()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedItem))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, raised);
            }
        }

        public class SelectedIndexes : ScopedTestBase
        {
            [Fact]
            public void PropertyChanged_Is_Raised_When_SelectedIndex_Changes()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndexes))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, raised);
            }

            [Fact]
            public void CollectionChanged_Is_Raised_When_SelectedIndex_Changes()
            {
                var target = CreateTarget();
                var raised = 0;
                var incc = Assert.IsAssignableFrom<INotifyCollectionChanged>(target.SelectedIndexes);

                incc.CollectionChanged += (s, e) =>
                {
                    // For the moment, for simplicity, we raise a Reset event when the SelectedIndexes
                    // collection changes - whatever the change. This can be improved later if necessary.
                    Assert.Equal(NotifyCollectionChangedAction.Reset, e.Action);
                    ++raised;
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, raised);
            }
        }

        public class SelectedItems : ScopedTestBase
        {
            [Fact]
            public void PropertyChanged_Is_Raised_When_SelectedIndex_Changes()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedItems))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, raised);
            }

            [Fact]
            public void CollectionChanged_Is_Raised_When_SelectedIndex_Changes()
            {
                var target = CreateTarget();
                var raised = 0;
                var incc = Assert.IsAssignableFrom<INotifyCollectionChanged>(target.SelectedIndexes);

                incc.CollectionChanged += (s, e) =>
                {
                    // For the moment, for simplicity, we raise a Reset event when the SelectedItems
                    // collection changes - whatever the change. This can be improved later if necessary.
                    Assert.Equal(NotifyCollectionChangedAction.Reset, e.Action);
                    ++raised;
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, raised);
            }
        }

        public class Select : ScopedTestBase
        {
            [Fact]
            public void Select_Sets_SelectedIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndex))
                    {
                        ++raised;
                    }
                };

                target.Select(1);

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Select_Clears_Old_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 0 }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "foo" }, e.DeselectedItems);
                    Assert.Equal(new[] { 1 }, e.SelectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.SelectedItems);
                    ++raised;
                };

                target.Select(1);

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Select_With_Invalid_Index_Does_Nothing()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 0;

                target.PropertyChanged += (s, e) => ++raised;
                target.SelectionChanged += (s, e) => ++raised;

                target.Select(5);

                Assert.Equal(0, target.SelectedIndex);
                Assert.Equal(new[] { 0 }, target.SelectedIndexes);
                Assert.Equal("foo", target.SelectedItem);
                Assert.Equal(new[] { "foo" }, target.SelectedItems);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Selecting_Already_Selected_Item_Doesnt_Raise_SelectionChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.Select(2);
                target.SelectionChanged += (s, e) => ++raised;
                target.Select(2);

                Assert.Equal(0, raised);
            }
        }

        public class SelectRange : ScopedTestBase
        {
            [Fact]
            public void SelectRange_Throws()
            {
                var target = CreateTarget();

                Assert.Throws<InvalidOperationException>(() => target.SelectRange(0, 10));
            }
        }

        public class Deselect : ScopedTestBase
        {
            [Fact]
            public void Deselect_Clears_Current_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 0 }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "foo" }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.Deselect(0);

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Deselect_Does_Nothing_For_Nonselected_Item()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 1;
                target.SelectionChanged += (s, e) => ++raised;
                target.Deselect(0);

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(0, raised);
            }
        }

        public class DeselectRange : ScopedTestBase
        {
            [Fact]
            public void DeselectRange_Clears_Current_Selection_For_Intersecting_Range()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 0 }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "foo" }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.DeselectRange(0, 2);

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void DeselectRange_Does_Nothing_For_Nonintersecting_Range()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 0;
                target.SelectionChanged += (s, e) => ++raised;
                target.DeselectRange(1, 2);

                Assert.Equal(0, target.SelectedIndex);
                Assert.Equal(new[] { 0 }, target.SelectedIndexes);
                Assert.Equal("foo", target.SelectedItem);
                Assert.Equal(new[] { "foo" }, target.SelectedItems);
                Assert.Equal(0, raised);
            }
        }

        public class Clear : ScopedTestBase
        {
            [Fact]
            public void Clear_Raises_SelectionChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.Select(1);

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 1 }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.Clear();

                Assert.Equal(1, raised);
            }
        }

        public class AnchorIndex : ScopedTestBase
        {
            [Fact]
            public void Setting_SelectedIndex_Sets_AnchorIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, target.AnchorIndex);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Setting_SelectedIndex_To_Minus_1_Doesnt_Clear_AnchorIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 1;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = -1;

                Assert.Equal(1, target.AnchorIndex);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Select_Sets_AnchorIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.Select(1);

                Assert.Equal(1, target.AnchorIndex);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Deselect_Doesnt_Clear_AnchorIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.Select(1);

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.Deselect(1);

                Assert.Equal(1, target.AnchorIndex);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Raises_PropertyChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = 1;

                Assert.Equal(1, raised);
            }
        }

        public class SingleSelect : ScopedTestBase
        {
            [Fact]
            public void Converting_To_Multiple_Selection_Preserves_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 1;

                target.SelectionChanged += (s, e) => ++raised;

                target.SingleSelect = false;

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Raises_PropertyChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SingleSelect))
                    {
                        ++raised;
                    }
                };

                target.SingleSelect = false;

                Assert.Equal(1, raised);
            }
        }

        public class CollectionChanges : ScopedTestBase
        {
            [Fact]
            public void Adding_Item_Before_Selected_Item_Updates_Indexes()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var selectionChangedRaised = 0;
                var indexesChangedRaised = 0;
                var selectedIndexRaised = 0;

                target.SelectedIndex = 1;

                target.SelectionChanged += (s, e) => ++selectionChangedRaised;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndex))
                    {
                        ++selectedIndexRaised;
                    }
                };

                target.IndexesChanged += (s, e) =>
                {
                    Assert.Equal(0, e.StartIndex);
                    Assert.Equal(1, e.Delta);
                    ++indexesChangedRaised;
                };

                data.Insert(0, "new");

                Assert.Equal(2, target.SelectedIndex);
                Assert.Equal(new[] { 2 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(2, target.AnchorIndex);
                Assert.Equal(1, indexesChangedRaised);
                Assert.Equal(1, selectedIndexRaised);
                Assert.Equal(0, selectionChangedRaised);
            }

            [Fact]
            public void Adding_Item_After_Selected_Doesnt_Raise_Events()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var raised = 0;

                target.SelectedIndex = 1;

                target.PropertyChanged += (s, e) => ++raised;
                target.SelectionChanged += (s, e) => ++raised;
                target.IndexesChanged += (s, e) => ++raised;

                data.Insert(2, "new");

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(1, target.AnchorIndex);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Removing_Selected_Item_Updates_State()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var selectionChangedRaised = 0;
                var selectedIndexRaised = 0;

                target.Source = data;
                target.Select(1);

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndex))
                    {
                        ++selectedIndexRaised;
                    }
                };

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++selectionChangedRaised;
                };

                data.RemoveAt(1);

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(-1, target.AnchorIndex);
                Assert.Equal(1, selectionChangedRaised);
                Assert.Equal(1, selectedIndexRaised);
            }

            [Fact]
            public void Removing_Item_Before_Selected_Item_Updates_Indexes()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var selectionChangedRaised = 0;
                var indexesChangedraised = 0;

                target.SelectedIndex = 1;

                target.SelectionChanged += (s, e) => ++selectionChangedRaised;

                target.IndexesChanged += (s, e) =>
                {
                    Assert.Equal(0, e.StartIndex);
                    Assert.Equal(-1, e.Delta);
                    ++indexesChangedraised;
                };

                data.RemoveAt(0);

                Assert.Equal(0, target.SelectedIndex);
                Assert.Equal(new[] { 0 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(0, target.AnchorIndex);
                Assert.Equal(1, indexesChangedraised);
                Assert.Equal(0, selectionChangedRaised);
            }

            [Fact]
            public void Removing_Item_After_Selected_Doesnt_Raise_Events()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var raised = 0;

                target.SelectedIndex = 1;

                target.PropertyChanged += (s, e) => ++raised;
                target.SelectionChanged += (s, e) => ++raised;
                target.IndexesChanged += (s, e) => ++raised;

                data.RemoveAt(2);

                Assert.Equal(1, target.SelectedIndex);
                Assert.Equal(new[] { 1 }, target.SelectedIndexes);
                Assert.Equal("bar", target.SelectedItem);
                Assert.Equal(new[] { "bar" }, target.SelectedItems);
                Assert.Equal(1, target.AnchorIndex);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Replacing_Selected_Item_Updates_State()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var selectionChangedRaised = 0;
                var selectedIndexRaised = 0;
                var selectedItemRaised = 0;

                target.Source = data;
                target.Select(1);

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndex))
                    {
                        ++selectedIndexRaised;
                    }

                    if (e.PropertyName == nameof(target.SelectedItem))
                    {
                        ++selectedItemRaised;
                    }
                };

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++selectionChangedRaised;
                };

                data[1] = "new";

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(-1, target.AnchorIndex);
                Assert.Equal(1, selectionChangedRaised);
                Assert.Equal(1, selectedIndexRaised);
                Assert.Equal(1, selectedItemRaised);
            }

            [Fact]
            public void Moving_Selected_Item_Updates_State()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var selectionChangedRaised = 0;
                var selectedIndexRaised = 0;
                var selectedItemRaised = 0;

                target.Source = data;
                target.Select(1);

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndex))
                    {
                        ++selectedIndexRaised;
                    }

                    if (e.PropertyName == nameof(target.SelectedItem))
                    {
                        ++selectedItemRaised;
                    }
                };

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.DeselectedItems);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++selectionChangedRaised;
                };

                data.Move(1, 0);

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(-1, target.AnchorIndex);
                Assert.Equal(1, selectionChangedRaised);
                Assert.Equal(1, selectedIndexRaised);
                Assert.Equal(1, selectedItemRaised);
            }

            [Fact]
            public void Resetting_Source_Updates_State()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var selectionChangedRaised = 0;
                var selectedIndexRaised = 0;
                var resetRaised = 0;

                target.Source = data;
                target.Select(1);

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndex))
                    {
                        ++selectedIndexRaised;
                    }
                };

                target.SelectionChanged += (s, e) => ++selectionChangedRaised;
                target.SourceReset += (s, e) => ++resetRaised;

                data.Clear();

                Assert.Equal(-1, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(-1, target.AnchorIndex);
                Assert.Equal(0, selectionChangedRaised);
                Assert.Equal(1, resetRaised);
                Assert.Equal(1, selectedIndexRaised);
            }

            [Fact]
            public void Handles_Selection_Made_In_CollectionChanged()
            {
                // Tests the following scenario:
                //
                // - Items changes from empty to having 1 item
                // - ViewModel auto-selects item 0 in CollectionChanged
                // - SelectionModel receives CollectionChanged
                // - And so adjusts the selected item from 0 to 1, which is past the end of the items.
                //
                // There's not much we can do about this situation because the order in which
                // CollectionChanged handlers are called can't be known (the problem also exists with
                // WPF). The best we can do is not select an invalid index.
                var target = CreateTarget(createData: false);
                var data = new AvaloniaList<string>();

                data.CollectionChanged += (s, e) =>
                {
                    target.Select(0);
                };

                target.Source = data;
                data.Add("foo");

                Assert.Equal(0, target.SelectedIndex);
                Assert.Equal(new[] { 0 }, target.SelectedIndexes);
                Assert.Equal("foo", target.SelectedItem);
                Assert.Equal(new[] { "foo" }, target.SelectedItems);
                Assert.Equal(0, target.AnchorIndex);
            }

            [Fact]
            public void SelectedItems_Indexer_Is_Correct()
            {
                // Issue #7974
                var target = CreateTarget();
                var raised = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal("bar", e.SelectedItems.First());
                    Assert.Equal("bar", e.SelectedItems[0]);
                    ++raised;
                };

                target.Select(1);
                Assert.Equal(1, raised);
            }
        }

        public class BatchUpdate : ScopedTestBase
        {
            [Fact]
            public void Changes_Do_Not_Take_Effect_Until_EndUpdate_Called()
            {
                var target = CreateTarget();

                target.BeginBatchUpdate();
                target.Select(0);

                Assert.Equal(-1, target.SelectedIndex);

                target.EndBatchUpdate();

                Assert.Equal(0, target.SelectedIndex);
            }

            [Fact]
            public void Correctly_Batches_Clear_SelectedIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 2;
                target.SelectionChanged += (s, e) => ++raised;

                using (target.BatchUpdate())
                {
                    target.Clear();
                    target.SelectedIndex = 2;
                }

                Assert.Equal(0, raised);
            }
        }

        public class LostSelection : ScopedTestBase
        {
            [Fact]
            public void LostSelection_Called_On_Clear()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 1;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 1 }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.DeselectedItems);
                    Assert.Equal(new[] { 0 }, e.SelectedIndexes);
                    Assert.Equal(new[] { "foo" }, e.SelectedItems);
                    ++raised;
                };

                target.LostSelection += (s, e) =>
                {
                    target.Select(0);
                };

                target.Clear();

                Assert.Equal(0, target.SelectedIndex);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void LostSelection_Called_When_SelectedItem_Removed()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var raised = 0;

                target.SelectedIndex = 1;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.DeselectedItems);
                    Assert.Equal(new[] { 0 }, e.SelectedIndexes);
                    Assert.Equal(new[] { "foo" }, e.SelectedItems);
                    ++raised;
                };

                target.LostSelection += (s, e) =>
                {
                    target.Select(0);
                };

                data.RemoveAt(1);

                Assert.Equal(0, target.SelectedIndex);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void LostSelection_Not_Called_With_Old_Source_When_Changing_Source()
            {
                var target = CreateTarget();
                var data = (AvaloniaList<string>)target.Source!;
                var raised = 0;

                target.LostSelection += (s, e) =>
                {
                    if (target.Source == data)
                    {
                        ++raised;
                    }
                };

                target.Source = null;

                Assert.Equal(0, raised);
            }

            [Fact]
            public void LostSelection_Is_Called_When_Source_Changed_While_CollectionChange_In_Progress()
            {
                // Issue #12733.
                var data1 = new AvaloniaList<string> { "foo1", "bar1", "baz1" };
                var data2 = new AvaloniaList<string> { "foo1", "bar1", "baz1" };
                var target = new DerivedSelectionModel { Source = data1 };
                var raised = 0;

                target.LostSelection += (s, e) =>
                {
                    if (target.Source == data2)
                    {
                        ++raised;
                    }
                };

                target.UpdateSource(data2);

                Assert.Equal(1, raised);
            }

            private class DerivedSelectionModel : SelectionModel<string?>
            {
                public void UpdateSource(IEnumerable? source)
                {
                    OnSourceCollectionChangeStarted();
                    Source = source;
                    OnSourceCollectionChangeFinished();
                }
            }
        }

        public class UntypedInterface : ScopedTestBase
        {
            [Fact]
            public void Raises_Untyped_SelectionChanged_Event()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = 1;

                ((ISelectionModel)target).SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { 1 }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "bar" }, e.DeselectedItems);
                    Assert.Equal(new[] { 2 }, e.SelectedIndexes);
                    Assert.Equal(new[] { "baz" }, e.SelectedItems);
                    ++raised;
                };

                target.SelectedIndex = 2;

                Assert.Equal(1, raised);
            }
        }

        private static SelectionModel<string?> CreateTarget(bool createData = true)
        {
            var result = new SelectionModel<string?> { SingleSelect = true };

            if (createData)
            {
                result.Source = new AvaloniaList<string> { "foo", "bar", "baz" };
            }

            return result;
        }
    }
}
