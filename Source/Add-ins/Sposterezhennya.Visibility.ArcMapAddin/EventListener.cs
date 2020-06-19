using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Editor;
using static MilSpace.Visibility.EventListener;

namespace MilSpace.Visibility
{
    public class EventListener
    {
        #region Enums
        //contains all edit events listed on IEditEvents through IEditEvents4
        public enum EditorEventEnum
        {
            AfterDrawSketch,
            OnChangeFeature,
            OnConflictsDetected,
            OnCreateFeature,
            OnCurrentLayerChanged,
            OnCurrentTaskChanged,
            OnDeleteFeature,
            OnRedo,
            OnSelectionChanged,
            OnSketchFinished,
            OnSketchModified,
            OnStartEditing,
            OnStopEditing,
            OnUndo,
            BeforeStopEditing,
            BeforeStopOperation,
            OnVertexAdded,
            OnVertexMoved,
            OnVertexDeleted,
            BeforeDrawSketch,
            OnAngularCorrectionOffsetChanged,
            OnDistanceCorrectionFactorChanged,
            OnUseGroundToGridChanged,
        };
        #endregion

        #region Constructors
        public EventListener(IEditor editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException();
            }

            m_editor = editor;

        }
        public EventListener(IEditor editor, EditorEventEnum editEvent)
        {
            if (editor == null)
            {
                throw new ArgumentNullException();
            }

            m_editor = editor;

        }
        EventListener(IEditor editor, bool bListenAll)
        {
            if (editor == null)
            {
                throw new ArgumentNullException();
            }

            m_editor = editor;
        }
        #endregion

        public void ListenToEvents(EditorEventEnum editEvent, bool start)
        {
            switch (editEvent)
            {
                case EditorEventEnum.AfterDrawSketch:
                    if (start)
                        ((IEditEvents_Event)m_editor).AfterDrawSketch += new
                          IEditEvents_AfterDrawSketchEventHandler(EventListener_AfterDrawSketch);
                    else
                        ((IEditEvents_Event)m_editor).AfterDrawSketch -= new
                          IEditEvents_AfterDrawSketchEventHandler(EventListener_AfterDrawSketch);
                    break;
                case EditorEventEnum.OnChangeFeature:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnChangeFeature += new
                          IEditEvents_OnChangeFeatureEventHandler(EventListener_OnChangeFeature);
                    else
                        ((IEditEvents_Event)m_editor).OnChangeFeature -= new
                          IEditEvents_OnChangeFeatureEventHandler(EventListener_OnChangeFeature);
                    break;
                case EditorEventEnum.OnConflictsDetected:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnConflictsDetected += new
                          IEditEvents_OnConflictsDetectedEventHandler(EventListener_OnConflictsDetected);
                    else
                        ((IEditEvents_Event)m_editor).OnConflictsDetected -= new
                          IEditEvents_OnConflictsDetectedEventHandler(EventListener_OnConflictsDetected);
                    break;
                case EditorEventEnum.OnCreateFeature:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnCreateFeature += new
                          IEditEvents_OnCreateFeatureEventHandler(EventListener_OnCreateFeature);
                    else
                        ((IEditEvents_Event)m_editor).OnCreateFeature -= new
                          IEditEvents_OnCreateFeatureEventHandler(EventListener_OnCreateFeature);
                    break;

                case EditorEventEnum.OnCurrentLayerChanged:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnCurrentLayerChanged += new
                          IEditEvents_OnCurrentLayerChangedEventHandler(EventListener_OnCurrentLayerChanged);
                    else
                        ((IEditEvents_Event)m_editor).OnCurrentLayerChanged -= new
                          IEditEvents_OnCurrentLayerChangedEventHandler(EventListener_OnCurrentLayerChanged);
                    break;
                case EditorEventEnum.OnCurrentTaskChanged:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnCurrentTaskChanged += new
                          IEditEvents_OnCurrentTaskChangedEventHandler(OnCurrentTaskChanged);
                    else
                        ((IEditEvents_Event)m_editor).OnCurrentTaskChanged -= new
                          IEditEvents_OnCurrentTaskChangedEventHandler(OnCurrentTaskChanged);
                    break;
                case EditorEventEnum.OnDeleteFeature:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnDeleteFeature += new
                          IEditEvents_OnDeleteFeatureEventHandler(EventListener_OnDeleteFeature);
                    else
                        ((IEditEvents_Event)m_editor).OnDeleteFeature -= new
                          IEditEvents_OnDeleteFeatureEventHandler(EventListener_OnDeleteFeature);
                    break;
                case EditorEventEnum.OnRedo:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnRedo += new
                          IEditEvents_OnRedoEventHandler(EventListener_OnRedo);
                    else
                        ((IEditEvents_Event)m_editor).OnRedo -= new
                          IEditEvents_OnRedoEventHandler(EventListener_OnRedo);
                    break;
                case EditorEventEnum.OnSelectionChanged:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnSelectionChanged += new
                          IEditEvents_OnSelectionChangedEventHandler(EventListener_OnSelectionChanged);
                    else
                        ((IEditEvents_Event)m_editor).OnSelectionChanged -= new
                          IEditEvents_OnSelectionChangedEventHandler(EventListener_OnSelectionChanged);
                    break;
                case EditorEventEnum.OnSketchFinished:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnSketchFinished += new
                          IEditEvents_OnSketchFinishedEventHandler(EventListener_OnSketchFinished);
                    else
                        ((IEditEvents_Event)m_editor).OnSketchFinished -= new
                          IEditEvents_OnSketchFinishedEventHandler(EventListener_OnSketchFinished);
                    break;
                case EditorEventEnum.OnSketchModified:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnSketchModified += new
                          IEditEvents_OnSketchModifiedEventHandler(EventListener_OnSketchModified);
                    else
                        ((IEditEvents_Event)m_editor).OnSketchModified -= new
                          IEditEvents_OnSketchModifiedEventHandler(EventListener_OnSketchModified);
                    break;
                case EditorEventEnum.OnStartEditing:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnStartEditing += new
                          IEditEvents_OnStartEditingEventHandler(OnStartEditing);
                    else
                        ((IEditEvents_Event)m_editor).OnStartEditing -= new
                          IEditEvents_OnStartEditingEventHandler(OnStartEditing);
                    break;
                case EditorEventEnum.OnStopEditing:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnStopEditing += new
                          IEditEvents_OnStopEditingEventHandler(OnStopEditing);
                    else
                        ((IEditEvents_Event)m_editor).OnStopEditing -= new
                          IEditEvents_OnStopEditingEventHandler(OnStopEditing);
                    break;
                case EditorEventEnum.OnUndo:
                    if (start)
                        ((IEditEvents_Event)m_editor).OnUndo += new
                          IEditEvents_OnUndoEventHandler(EventListener_OnUndo);
                    else
                        ((IEditEvents_Event)m_editor).OnUndo -= new
                          IEditEvents_OnUndoEventHandler(EventListener_OnUndo);
                    break;
                case EditorEventEnum.BeforeStopEditing:
                    if (start)
                        ((IEditEvents2_Event)m_editor).BeforeStopEditing += new
                          IEditEvents2_BeforeStopEditingEventHandler(EventListener_BeforeStopEditing);
                    else
                        ((IEditEvents2_Event)m_editor).BeforeStopEditing -= new
                          IEditEvents2_BeforeStopEditingEventHandler(EventListener_BeforeStopEditing);
                    break;
                case EditorEventEnum.BeforeStopOperation:
                    if (start)
                        ((IEditEvents2_Event)m_editor).BeforeStopOperation += new
                          IEditEvents2_BeforeStopOperationEventHandler(EventListener_BeforeStopOperation);
                    else
                        ((IEditEvents2_Event)m_editor).BeforeStopOperation -= new
                          IEditEvents2_BeforeStopOperationEventHandler(EventListener_BeforeStopOperation);
                    break;
                case EditorEventEnum.OnVertexAdded:
                    if (start)
                        ((IEditEvents2_Event)m_editor).OnVertexAdded += new
                          IEditEvents2_OnVertexAddedEventHandler(EventListener_OnVertexAdded);
                    else
                        ((IEditEvents2_Event)m_editor).OnVertexAdded -= new
                          IEditEvents2_OnVertexAddedEventHandler(EventListener_OnVertexAdded);
                    break;
                case EditorEventEnum.OnVertexMoved:
                    if (start)
                        ((IEditEvents2_Event)m_editor).OnVertexMoved += new
                          IEditEvents2_OnVertexMovedEventHandler(EventListener_OnVertexMoved);
                    else
                        ((IEditEvents2_Event)m_editor).OnVertexMoved -= new
                          IEditEvents2_OnVertexMovedEventHandler(EventListener_OnVertexMoved);
                    break;
                case EditorEventEnum.OnVertexDeleted:
                    if (start)
                        ((IEditEvents2_Event)m_editor).OnVertexDeleted += new
                          IEditEvents2_OnVertexDeletedEventHandler(EventListener_OnVertexDeleted);
                    else
                        ((IEditEvents2_Event)m_editor).OnVertexDeleted -= new
                          IEditEvents2_OnVertexDeletedEventHandler(EventListener_OnVertexDeleted);
                    break;
                case EditorEventEnum.BeforeDrawSketch:
                    if (start)
                        ((IEditEvents3_Event)m_editor).BeforeDrawSketch += new
                          IEditEvents3_BeforeDrawSketchEventHandler(EventListener_BeforeDrawSketch);
                    else
                        ((IEditEvents3_Event)m_editor).BeforeDrawSketch -= new
                          IEditEvents3_BeforeDrawSketchEventHandler(EventListener_BeforeDrawSketch);
                    break;
                case EditorEventEnum.OnAngularCorrectionOffsetChanged:
                    if (start)
                        ((IEditEvents4_Event)m_editor).OnAngularCorrectionOffsetChanged += new
                          IEditEvents4_OnAngularCorrectionOffsetChangedEventHandler(EventListener_OnAngularCorrectionOffsetChanged);
                    else
                        ((IEditEvents4_Event)m_editor).OnAngularCorrectionOffsetChanged -= new
                          IEditEvents4_OnAngularCorrectionOffsetChangedEventHandler(EventListener_OnAngularCorrectionOffsetChanged);
                    break;
                case EditorEventEnum.OnDistanceCorrectionFactorChanged:
                    if (start)
                        ((IEditEvents4_Event)m_editor).OnDistanceCorrectionFactorChanged += new
                          IEditEvents4_OnDistanceCorrectionFactorChangedEventHandler(EventListener_OnDistanceCorrectionFactorChanged);
                    else
                        ((IEditEvents4_Event)m_editor).OnDistanceCorrectionFactorChanged -= new
                          IEditEvents4_OnDistanceCorrectionFactorChangedEventHandler(EventListener_OnDistanceCorrectionFactorChanged);
                    break;
                case EditorEventEnum.OnUseGroundToGridChanged:
                    if (start)
                        ((IEditEvents4_Event)m_editor).OnUseGroundToGridChanged += new
                          IEditEvents4_OnUseGroundToGridChangedEventHandler(EventListener_OnUseGroundToGridChanged);

                    else
                        ((IEditEvents4_Event)m_editor).OnUseGroundToGridChanged -= new
                          IEditEvents4_OnUseGroundToGridChangedEventHandler(EventListener_OnUseGroundToGridChanged);
                    break;
                default:
                    break;

            }

        }

        // Invoke the Changed event
        protected virtual void OnChanged(EditorEventArgs e)
        {
            Changed?.Invoke(this, e);
        }

        #region Event Handlers
        void EventListener_OnCreateFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnCreateFeature);
            e.Object = obj;
            OnChanged(e);
        }
        void EventListener_OnChangeFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnChangeFeature);
            e.Object = obj;
            OnChanged(e);
        }
        void EventListener_OnConflictsDetected()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnConflictsDetected);
            OnChanged(e);
        }
        void EventListener_OnCurrentLayerChanged()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnCurrentLayerChanged);
            OnChanged(e);
        }
        void EventListener_OnDeleteFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnDeleteFeature);
            OnChanged(e);
        }
        void EventListener_OnRedo()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnRedo);
            OnChanged(e);
        }
        void EventListener_OnSelectionChanged()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnSelectionChanged);
            OnChanged(e);
        }
        void EventListener_OnSketchFinished()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnSketchFinished);
            OnChanged(e);
        }
        void EventListener_OnSketchModified()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnSketchModified);
            OnChanged(e);
        }
        void EventListener_OnUndo()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnUndo);
            OnChanged(e);
        }
        void EventListener_BeforeStopEditing(bool save)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.BeforeStopEditing);
            OnChanged(e);
        }
        void EventListener_BeforeStopOperation()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.BeforeStopOperation);
            OnChanged(e);
        }
        void EventListener_OnVertexAdded(ESRI.ArcGIS.Geometry.IPoint point)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnVertexAdded);
            OnChanged(e);
        }
        void EventListener_OnVertexMoved(ESRI.ArcGIS.Geometry.IPoint point)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnVertexMoved);
            OnChanged(e);
        }
        void EventListener_OnVertexDeleted(ESRI.ArcGIS.Geometry.IPoint point)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnVertexDeleted);
            OnChanged(e);
        }
        void EventListener_OnAngularCorrectionOffsetChanged(double angOffset)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnAngularCorrectionOffsetChanged);
            OnChanged(e);
        }
        void EventListener_OnDistanceCorrectionFactorChanged(double distFactor)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnDistanceCorrectionFactorChanged);
            OnChanged(e);
        }
        void EventListener_OnUseGroundToGridChanged(bool g2g)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnUseGroundToGridChanged);
            OnChanged(e);
        }
        void EventListener_BeforeDrawSketch(ESRI.ArcGIS.Display.IDisplay pDpy)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.BeforeDrawSketch);
            OnChanged(e);
        }
        void EventListener_AfterDrawSketch(ESRI.ArcGIS.Display.IDisplay pDpy)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.AfterDrawSketch);
            OnChanged(e);
        }
        void OnCurrentTaskChanged()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnCurrentTaskChanged);
            OnChanged(e);
        }
        void OnStopEditing(bool SaveEdits)
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnStopEditing);
            e.SaveEdits = SaveEdits;
            OnChanged(e);
        }
        void OnStartEditing()
        {
            EditorEventArgs e = new EditorEventArgs(EditorEventEnum.OnStartEditing);
            OnChanged(e);
        }
        #endregion

        #region Members

        public event ChangedEventHandler Changed;

        IEditor m_editor;
        #endregion
    }

    public class EditorEventArgs : EventArgs
    {
        public EditorEventArgs(EditorEventEnum eventType)
        {
            this.EventType = eventType;
        }
        public EditorEventEnum EventType;

        public ESRI.ArcGIS.Geodatabase.IObject Object;

        public bool? SaveEdits;
    }

    public delegate void ChangedEventHandler(object sender, EditorEventArgs e);
}
