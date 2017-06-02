// <script>

/*
=============================================================
WebIntelligence(r) Report Panel
Copyright(c) 2001-2003 Business Objects S.A.
All rights reserved

Use and support of this software is governed by the terms
and conditions of the software license agreement and support
policy of Business Objects S.A. and/or its subsidiaries.
The Business Objects products and technology are protected
by the US patent number 5,555,403 and 6,247,008

File: treeview.js
Custom treeview control
=============================================================
*/



// ================================================================================
// ================================================================================
//
// OBJECT newIconListWidget (Constructor)
//
// List box with icons, based on tree widget
//
// ================================================================================
// ================================================================================
/*
function newIconListWidget(id,w,h,icns,clickCB,doubleClickCB,bgClass)
// CONSTRUCTOR
// id            [String] the id for DHTML processing
// w             [int] the tree wiew frame width in pixels
// h             [int] the tree wiew frame height in pixels
// icn           [String] the combined icon URL
// clickCB       [Function] callback called when clicking on an item
// doubleClickCB [Function] callback called when double clicking on an item
{
	var o=newTreeWidget(id,w,h,icns,clickCB,doubleClickCB,bgClass)

	o.select=IconListWidget_select
	o.getSelection=IconListWidget_getSelection
	o.initialIndent=-_trIndent+3

	return o
}

// ================================================================================

function IconListWidget_select(pos,setFocus,ev,noSendClickCB,isFromKeyArrow)
// Select by position
// pos [int] : the position
// returns [void]
{
	if ((pos>=0) && (pos<this.sub.length)) {
		this.sub[pos].select(setFocus,ev,noSendClickCB,isFromKeyArrow)
	}
}

// ================================================================================

function IconListWidget_getSelection()
// Get selction
// returns [object] index, value
{
	var sel=this.getSelectedItem()
	if (sel)
	{
		if (sel.elemPos==-1) this.buildElems()
		var selection=new Object
		selection.index=sel.elemPos
		selection.value=sel.userData
		return selection
	}
	return null
}

// ================================================================================
// ================================================================================
//
// OBJECT newIconListPopupWidget (Constructor)
//
// List box with icons, based on tree widget show in popup like a menu
//
// ================================================================================
// ================================================================================

function newIconListPopupWidget(id,w,h,icns,clickCB,doubleClickCB)
// CONSTRUCTOR
// id            [String] the id for DHTML processing
// w             [int] the tree wiew frame width in pixels
// h             [int] the tree wiew frame height in pixels
// icn           [String] the combined icon URL
// clickCB       [Function] callback called when clicking on an item
// doubleClickCB [Function] callback called when double clicking on an item
{
	var o=newWidget(id)

	o.iconList=newIconListWidget("list_"+id,w,h,icns,clickCB,doubleClickCB,"treeNoBorder")
	o.getList=IconListPopupWidget_getList
	//o.init=IconListPopupWidget_init
	o.getHTML=IconListPopupWidget_getHTML
	o.justInTimeInit=IconListPopupWidget_justInTimeInit
	o.getShadowHTML=IconListPopupWidget_getShadowHTML
	o.show=IconListPopupWidget_show	
	o.captureClicks=IconListPopupWidget_captureClicks
	o.releaseClicks=IconListPopupWidget_releaseClicks
	o.getSelection=IconListPopupWidget_getSelection
	
	// Click capture
	o.clickCB=new Array
	o.clickCBDocs=new Array
	
	//cancelCB
	o.cancelCB=null;

	return o
}

// ================================================================================
function IconListPopupWidget_init()
{
	var o=this
	o.iconList.init();
	o.init();
}
// ================================================================================
function IconListPopupWidget_getList()
{
	return this.iconList;
}
// ================================================================================
function IconListPopupWidget_getSelection()
{
}
// ================================================================================
function IconListPopupWidget_getShadowHTML()
// Displays the shadow of the IconListPopup
// Return [void]
{
	return getBGIframe('menuIframe_'+this.id)
}

// ================================================================================
function IconListPopupWidget_getHTML()
// Return [string]	the HTML sorce of this widget
{
	var o=this
	var keyCB=' onkeydown="'+_codeWinName+'.IconListPopupWidget_keyDown(\''+o.id+'\',event)" onkeypress=" return '+_codeWinName+'.IconListPopupWidget_keyPress(\''+o.id+'\',event)" onkeyup="'+_codeWinName+'.IconListPopupWidget_keyUp(\''+o.id+'\',event)"';	
	var s=''
	s+=o.getShadowHTML()
	s+='<table id="'+o.id+'" style="display:none;" class="menuFrame" cellspacing="0" cellpadding="0" border="0" '+keyCB+'><tbody>'
	s+='<tr><td align="center">'+o.iconList.getHTML()+'</td></tr>'	
	s+='</tbody></table>'
	
	return s
}

// ================================================================================
function IconListPopupWidget_justInTimeInit()
// Initialization called just before showing scrollMenu for the first time
// Return [void]
{
	var o=this
	o.layer=getLayer(o.id)
	
	if (o.layer==null)
	{
		targetApp(o.getHTML())
		o.layer=getLayer(o.id)
	}

	o.layer._widget=o.widx
	o.css=o.layer.style
	o.css.visibility="hidden"
	
	o.iframeLyr=getLayer("menuIframe_"+o.id)
	o.iframeCss=o.iframeLyr.style

	o.iconList.init()
	o.iconList.layer.onmousedown=IconListPopupWidget_clickNoBubble	
}

// ================================================================================
function IconListPopupWidget_show(show,x,y)
// show		[boolean] if true shows the menu, otherwise hide it
// x        [int] menu abscissa (may be changed if the menu is outside the window)
// y        [int] menu ordinate (may be changed if the menu is outside the window)
// Return	[void]
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()

	var css=o.css,iCss=o.iframeCss
	
	if (show)
	{		
		o.captureClicks()
	
		// Show and place menu	
		css.display='block'
		//css.zIndex=(o.zIndex+1)
		css.zIndex=4000
		css.visibility="hidden"
		css.left="-1000px"
		css.top="-1000px"
		
		var w=o.getWidth()
		var h=o.getHeight()
		
		if (o.alignLeft)
			x-=w
	
		// Change coordinates if the menu is out of the window
		var x2=x+w+4,y2=y+h+4

		if (x2>winWidth())
			x=Math.max(0,x-4-w)

		if (y2>winHeight())
			y=Math.max(0,y-4-h)
	
		css.left=""+x+"px"
		css.top=""+y+"px"
		
		//hideAllInputs(x,y,w+4,h+4)
		
		css.visibility="visible"

		// Show and place menu shadow

		iCss.left=""+x+"px"
		iCss.top=""+y+"px"
		iCss.width=""+w+"px"
		iCss.height=""+h+"px"
		iCss.zIndex=3998
		iCss.display='block'
		
		if (_ie)
		{
			y-=2
			x-=2
		}
			
	}
	else
	{	
		css.display='none'
		iCss.display='none'
	
		o.releaseClicks()
	}
}

// ================================================================================

function IconListPopupWidget_captureClicks(w)
// capture click in current frame and sub-frame, so when the users clicks outside
// the menu, the menu is closed. Eventual click handlers are stored.
// Use releaseClicks() to restore previous click handlers
// Returns [void]
{
	var o=this
	if (o.par==null)
	{
		if (w==null)
		{
			_globMenuCaptured=o
			o.clickCB.length=0
			o.clickCBDocs.length=0
			w=_curWin
		}

		//_excludeFromFrameScan variable is set by a frame that does not want to
		// be scaned, when the frame is used to download document for instance.

		if (canScanFrames(w))
		{
			if (_moz)
			{
				_oldErrHandler=window.onerror
				window.onerror=localErrHandler
			}
	
			try
			{
				d=w.document
				o.clickCB[o.clickCB.length]=d.onmousedown
				o.clickCBDocs[o.clickCBDocs.length]=d
				d.onmousedown=IconListPopupWidget_globalClick
			
				var fr=w.frames,len=fr.length
				for (var i=0;i<len;i++)
					o.captureClicks(fr[i])
			}
			catch(expt)
			{
			}

			if (_moz)
				window.onerror=_oldErrHandler
		}
	}
}

// ================================================================================

function IconListPopupWidget_globalClick()
// PRIVATE
// Click handler that close the menu if the user clicks outside the menu
// Also call releaseClicks() to restore previous click handlers
{
	var o=_globMenuCaptured
	if (o!=null)
	{
		_globMenuCaptured=null
		o.releaseClicks()
		o.show(false)
	}
}
// ================================================================================

function IconListPopupWidget_releaseClicks()
// Restore click handlers overrided by captureClicks()
// Returns [void]
{
	var o=this
	if (o.par==null)
	{
		var len=o.clickCB.length
		for (var i=0;i<len;i++)
		{
			o.clickCBDocs[i].onmousedown=o.clickCB[i]
			o.clickCB[i]=null
			o.clickCBDocs[i]=null
		}
		o.clickCB.length=0
		o.clickCBDocs.length=0
	}
}

// ================================================================================
function IconListPopupWidget_clickNoBubble(e)
{
	eventCancelBubble(e)
}
// ================================================================================
//NOTE: these 3 functions are usefull to do not propagate the return carriage for textarea (variableeditordialog.html)
function IconListPopupWidget_keyDown(id,e)
{
	var o=getWidget(getLayer(id))
	var key=eventGetKey(e)	
	if(key == 27)
	{		
		if(o && o.cancelCB)
			o.cancelCB(e)
	}
	if(key == 13)
	{	
		eventCancelBubble(e)
		return false
	}	
}
		
function IconListPopupWidget_keyPress(id,e)
{
	var o=getWidget(getLayer(id))
	var key=eventGetKey(e)
	if(key == 13 || key == 27)
	{	
		eventCancelBubble(e)
		return false
	}	
}

function IconListPopupWidget_keyUp(id,e)
{	
	var key=eventGetKey(e)	
	if(key == 13 || key == 27)
	{				
		eventCancelBubble(e)
		return false
	}	
}
*/

// ================================================================================
// ================================================================================
//
// OBJECT newTreeWidget (Constructor)
//
// Tree view class
//
// ================================================================================
// ================================================================================

var _trIndent=18
var _TreeWidgetElemInstances=new Array();

function newTreeWidget(id,w,h,icns,clickCB,doubleClickCB,bgClass,expandCB,collapseCB,deleteCB,minIcon,plusIcon)
// CONSTRUCTOR
// id            [String] the id for DHTML processing
// w             [int] the tree wiew frame width in pixels
// h             [int] the tree wiew frame height in pixels
// icn           [String] the combined icon URL, null if tree without icons
// clickCB       [Function] callback called when clicking on an item
// doubleClickCB [Function] callback called when double clicking on an item
// bgClass       [String - optional] CSS class for tree background and frame
// minIcon       [String - optional] URL to min.gif
// plusIcon      [String - optional] URL to plus.gif
{
	var o=newScrolledZoneWidget(id,2,4,w,h,bgClass)

	o.icns=icns
	o.sub = new Array
	o.clickCB=clickCB
	o.doubleClickCB=doubleClickCB
	o.expandCB=expandCB
	o.collapseCB=collapseCB
	o.deleteCB=deleteCB
	o.minIcon = minIcon
	o.plusIcon = plusIcon
	o.mouseOverCB=null
	
	o.rightClickMenuCB=null // need to call setRightClickMenu method
	
	o.mouseOverTooltip=false

	o.dragDrop=null

	o.oldInit=o.init
	o.init=TreeWidget_init
	o.getHTML=TreeWidget_getHTML
	
	o.getSelections=TreeWidget_getSelections
	// deprecated use getSelections()
	o.getSelectedItem=TreeWidget_getSelectedItem
	o.getSelectedItems=TreeWidget_getSelectedItems
	o.getCheckedItems=TreeWidget_getCheckedItems
	
	o.setDragDrop=TreeWidget_setDragDrop
	o.setFocus=TreeWidget_setFocus
	o.add=TreeWidget_add
	o.setRightClickMenuCB=TreeWidget_setRightClickMenuCB

	o.findByData=TreeWidget_findByData
	o.findById=TreeWidget_findById
	o.selectByData=TreeWidget_selectByData
	o.selectById=TreeWidget_selectById
	o.unselect=TreeWidget_unselect
//	o.search=TreeWidget_search

	o.treeLyr=null
	o.elemCount=0
	o.selId=-1;
	o.selIds=new Array; 
	o.multiSelection=false;
	o.hlPath=false; //highlight path
	o.hlElems=new Array; 
	o.iconOrientVertical=true
	o.focusNode=null;

	o.deleteAll=TreeWidget_deleteAll
	o.rebuildHTML=TreeWidget_rebuildHTML

	o.iconW=16
	o.iconH=16
	o.initialIndent=0

	o.getCount=TreeWidget_getCount

	o.dispIcnFuncName="dispIcn"
	
	o.setTooltipOnMouseOver=TreeWidget_setTooltipOnMouseOver
	o.setMouseOverCB=TreeWidget_setMouseOverCB
	
	o.setMultiSelection=TreeWidget_setMultiSelection
	o.setHighlightPath=TreeWidget_setHighlightPath
	o.highlightPath=TreeWidget_highlightPath
	o.unhlPath=TreeWidget_unhlPath
	
	o.getFirst=TreeWidget_getFirst
	o.getLast=TreeWidget_getLast

	return o
}

// ================================================================================

function TreeWidget_unselect()
// Unselect in the tree view
{
	var o=this	
	if (o.selId>=0)
	{
		var prev=_TreeWidgetElemInstances[o.selId]
		prev.unselect()
		o.selId=-1
	}
	if(o.multiSelection)
	{
		var len=o.selIds.length, id;
		for(var i=len-1;i>=0;i--)
		{
			var prev=_TreeWidgetElemInstances[o.selIds[i]]
			if(prev) prev.unselect()
		}
		o.selIds.length=0;
		o.layer._BOselIds="";
	}
	//unhighligh if necessary
	o.unhlPath()
}

// ================================================================================

function TreeWidget_selectByData(data,setFocus)
// Select an item that matches the data parameter
// data [Variant] the item data to find
// id [setFocus - Optional] Scroll to the item & set the focus on it
// return void
{
	var o=this,item=o.findByData(data)
	if (item)
	{
		item.select(setFocus)
	}
}

// ================================================================================

function TreeWidget_selectById(id,setFocus)
// Select an item that matches the id parameter
// id [String] the item id to find
// id [setFocus - Optional] Scroll to the item & set the focus on it
// return void
{
	var o=this,item=o.findById(id)
	if (item)
	{
		item.select(setFocus)
	}
}

// ================================================================================

function TreeWidget_findByData(data)
{
	var o=this,sub=o.sub,item=null

	for (var i = 0; i < sub.length; i++)
	{
		item=sub[i].findByData(data)
		if (item)
		    return item
	}
	return null
}

// ================================================================================

function TreeWidget_findById(id)
{
	var o=this,sub=o.sub,item=null

	for (var i = 0; i < sub.length; i++)
	{
		item=sub[i].findById(id)
		if (item)
		    return item
	}
	return null
}
// ================================================================================
//function TreeWidget_findInName(text,matchCase,matchWholeW,startFrom,next,starWith,visible)
////text [String] text to search in treeview
////matchCase [Boolean - Optional] if null or false then case is no sensitive
////matchWholeW [Boolean - Optional] if null or false then no match whole word
////startFrom [String - Optional] 2 options "begin" "end", by default we start from the first selection and ignore this parameter, if no selection then this parameter is used
////next [Boolean - Optional] search next or previous, by default it is next
////starWith [Boolean - Optional] search object begenning with this text, by default it is false
////visible [Boolean - Optional] search object in the deployed node (not in the hidden nodes), by default the search is performed in the entire tree even in hidden nodes
//{
//	//safe test
//	if(text=="" || text==null) return null; 
//	
//	var o=this,item=null,elem=null,hidden=false;
//	
//	var startPos=0,newPos=0;	
//	var bMatchCase=matchCase?matchCase:false;
//	var bMatchWW=matchWholeW?matchWholeW:false;	
//	var bNext=(!next)?next:true;
//	var bVisible=visible?visible:false;	
//	
//	var len=o.elems.length;
//	if(len == 0)
//	{ 
//		o.buildElems();	
//		len=o.elems.length;
//		if( len == 0) return;
//	}
//	//get the position of the selection	
//	var arr = o.getSelections();
//	if(arr.length>0) 
//	{
//		startPos=arr[0].elemPos+(bNext?1:-1);
//		
//		//verify that the start position is good
//		if((startPos<0) &&! bNext )
//			startPos=len-1;
//			
//		if((startPos==len) && bNext )
//			startPos=0;
//	}
//	else if(startFrom=="begin")
//	{
//		startPos=0;
//	}
//	else if(startFrom=="end")
//	{
//		startPos=len-1;
//	}
//	
//	newPos=startPos;
//	
//	while ((newPos>=0)&&(newPos<len))
//	{	
//		elem=o.elems[newPos];
//		hidden=elem.getHiddenParent();//not visible item			
//		if((bVisible && !hidden) || (!bVisible))
//			item=elem.findInName(text,bMatchCase,bMatchWW,bNext,starWith);
//		
//		if(item!=null) break;
//		
//		newPos=newPos+(bNext?1:-1);
//		
//		if((newPos<0) && !bNext )
//			newPos=len-1;
//			
//		if((newPos==len) && bNext )
//			newPos=0;		
//		
//		if(newPos==startPos) break;
//	}
//		
//	return item;
//}
// ================================================================================
//function TreeWidget_search(text,matchCase,matchWholeW,startFrom,next,notFoundCB,starWith,visible,setFocus)
////text [String] text to search in treeview
////matchCase [Boolean - Optional] if null or false then case is no sensitive
////matchWholeW [Boolean - Optional] if null or false then no match whole word
////startFrom [String - Optional] 2 options "begin" "end", by default we start from the first selection and ignore this parameter, if no selection then this parameter is used
////next [Boolean - Optional] search next or previous, by default it is next
////notFoundCB [Callback - Optional] call when no occurence is found
////starWith [Boolean - Optional] search object begenning with this text, by default it is false
////visible [Boolean - Optional] search object in the deployed node (not in the hidden nodes), by default the search is performed in the entire tree even in hidden nodes
////setFocus [Boolean - Optional] set the focus to the found item, by default it is false
//{
//	var o=this,item=null;
//	
//	if(text=="" || text==null) return ; 
//	
//	item = o.findInName(text,matchCase,matchWholeW,startFrom,next,starWith,visible);
//	
//	if(item)
//	{		
//		o.unselect();
//		item.select(setFocus);
//	}
//	else if(notFoundCB)
//	{
//		notFoundCB();
//	}				
//}

// ================================================================================

function TreeWidget_add(elem,extraIndent)
// Add an item in the tree
// elem [TreeWidgetElem] the tree view elemnt to add
// extraIndent [int - optional] the element additional indent
// returns [TreeWidgetElem] elem
{
	var o=this,sub=o.sub,len=sub.length
	elem.treeView=o
	elem.index=len
	sub[len]=elem
	elem.expanded=(len==0)
	if (extraIndent)
		elem.extraIndent=extraIndent

	return elem
}

// ================================================================================

function TreeWidget_getHTML()
// get the widget HTML
// returns [String] the HTML
{
	var o=this,sub=o.sub,len=sub.length,a=new Array(len+3),j=0

	a[j++]= o.beginHTML()+'<div id="treeCont_'+o.id+'" role="tree" tabindex="-1" onfocus="'+_dhtmlLib+'.TreeWidget_focusCB(this)">'
	for (var i = 0; i < sub.length; i++)
		a[j++]=sub[i].getHTML(o.initialIndent,i==0)
	a[j++]='</div>'+o.endHTML()

	return a.join("")
}

// ================================================================================

function TreeWidget_deleteAll()
// returns [void]
{
	var sub=this.sub
	for (var i = 0; i < sub.length; i++)
	{
		sub[i].deleteAll()
		sub[i]=null
	}
	sub.length=0
}

// ================================================================================

function TreeWidget_rebuildHTML()
// Rebuild completly the tree view
// returns [void]
{
	var o=this,sub=o.sub,len=sub.length,a=new Array(len),j=0,idt=o.initialIndent
	for (var i = 0; i < sub.length; i++)
		a[j++]=sub[i].getHTML(idt,i==0)
	o.treeLyr.innerHTML=a.join("")

	o.selId=-1
	o.layer._BOselId=-1
	
	//multi selection init
	o.selIds.length=0
	o.layer._BOselIds=""
}

// ================================================================================

function TreeWidget_init()
// Standard init function
// returns [void]
{
	this.oldInit();
	var l=this.treeLyr=getLayer('treeCont_'+this.id);
	
	//l.onkeydown=TreeWidget_keyDownCB;
	//l.onkeypress=TreeWidget_keyPressCB;	
	
	if (this.dragDrop)
		this.dragDrop.attachCallbacks(this.layer)

	var oldSel = this.layer._BOselId
	if (oldSel!=null)
		this.selId=oldSel
	
	//multi selection init
	var oldArraySel = this.layer._BOselIds; //string
	if (oldArraySel!=null && oldArraySel!="")
	{
		this.selIds.length=0;
		this.selIds=oldArraySel.split(";");
	}
	
	var sub=this.sub
}

// ================================================================================

function TreeWidget_getSelectedItem()
// deprecated use getSelections()
// Get the selected item
// Returns  [TreeWidgetElem] the selected itel (null if no selection)
{
	var id=this.selId
	return (id>=0)?_TreeWidgetElemInstances[id]:null
}

// ================================================================================
function TreeWidget_getSelections()
//unify interface for multi or single selection in treeview
{
	var o=this;
	
	if(o.multiSelection)
	{
		return o.getSelectedItems();
	}
	else
	{		
		var sel=o.getSelectedItem(),arrSel=new Array;		
		if(sel!=null) arrSel[0]=sel;
		
		return arrSel;		
		
		
	}
}
// ================================================================================

function TreeWidget_setFocus(index)
{
	var elem=_TreeWidgetElemInstances[index]
	if(elem!=null)
	{
		elem.init();
		
		var focus=this.focusNode;
		if(focus && focus.domElem) {
			focus.domElem.setAttribute("tabIndex", -1);
		}
		this.focusNode=elem;
		elem.domElem.setAttribute("tabIndex", 0);
	    
		try {
			elem.domElem.focus();
		}
		catch (e) {
			if (_ie9Up) throw e;
		}
	}
}
// ================================================================================
//function TreeWidget_keyPressCB(lay,e)
//// Internal key press event callback
//// Internal Note: Do not forget to fix a bug in single and multi selection functions.
//{
//	//multi selection treeview
////	if(getWidget(lay).multiSelection)
////	{ 
////		return TreeWidget_multiSelKeyPress(lay,e);		
////	}
//	
//	//mono selection treeview
//	var id=getWidget(lay).selId;
//	if (id>=0)
//	{
//		var elem=_TreeWidgetElemInstances[id]
//		var treeView=elem.treeView
//		var source =TreeIdToIdx(_ie?_curWin.event.srcElement:e.target)
//		
//		//_ie?_curWin.event.srcElement.id:e.target.id
//
//		var k=eventGetKey(e) , ctrl=_ie?_curWin.event.ctrlKey:e.ctrlKey
//		//window.status='press touche '+k + '('+ (t) +')'; t=t+1;
//		if( k==13 )
//		{					
//			//if(source!=_codeWinName+"trLstElt"+id)	//select the item if not selected
//			if(source!=id)	//select the item if not selected
//			{
//				TreeWidget_clickCB(source,false,null);
//				TreeWidgetElem_UpdateTooltip(source,true);
//			}
//			//else if ((source==_codeWinName+"trLstElt"+id)&&(treeView.doubleClickCB))
//			else if ((source==id)&&(treeView.doubleClickCB))
//				treeView.doubleClickCB(elem.userData);
//		}
//		if((k==10) && ctrl &&(source==_codeWinName+"trLstElt"+id))
//		{
//			if(elem.sub.length>0)
//			{
//				TreeWidget_toggleCB(id);
//				TreeWidgetElem_UpdateTooltip(source);
//			}	
//				
//			if (elem.isIncomplete&&elem.querycompleteCB)
//			{
//				elem.querycompleteCB()
//				TreeWidgetElem_UpdateTooltip(source);
//			}
//				
//			return false
//		}		
//	}
//	else //no selection in tree view
//	{
//		//var source =_ie?_curWin.event.srcElement.id:e.target.id
//		var source =TreeIdToIdx(_ie?_curWin.event.srcElement:e.target)
//		var k=eventGetKey(e);		
//		if( k==13 )
//		{						
//			TreeWidget_clickCB(source,false,null);
//			TreeWidgetElem_UpdateTooltip(source,true);			
//		}
//	}
//}
// ================================================================================
//
//function TreeWidget_multiSelKeyPress(o,e)
//{
//	//multi selection treeview
//	var treeView = getWidget(o);
//	var len = treeView.selIds.length;
//	if (len>0) //already has selection
//	{
//		//var source =_ie?_curWin.event.srcElement.id:e.target.id
//		var source =TreeIdToIdx(_ie?_curWin.event.srcElement:e.target)
//
//		var k=eventGetKey(e) , ctrl=_ie?_curWin.event.ctrlKey:e.ctrlKey
//		
//		//find the source in the selected items
//		var elem = null;
//		for(var i=0; i<len;i++)
//		{
//			var id = treeView.selIds[i];
//			//if (source==_codeWinName+"trLstElt"+id)
//			if (source==id)
//			{
//				elem = _TreeWidgetElemInstances[id];
//				break;
//			}
//		}
//		//IMPORTANT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//		//doubleClickCB will only send userData of the elem where the action is performed
//		//code must be added by the user in the doubleClickCB to manage the action for all the selected items
//		//please refer to sample treeview.html for more details
//		if( k==13 )//enter
//		{									
//			//TreeWidget_clickCB(source.slice(8+_codeWinName.length),false,null);
//			if(elem == null)
//			{
//				//TreeWidget_clickCB(source.slice(8+_codeWinName.length),false,_ie?_curWin.event:e);
//				TreeWidget_clickCB(source,false,_ie?_curWin.event:e);
//				TreeWidgetElem_UpdateTooltip(source,true);
//			}
//			else if (elem &&(treeView.doubleClickCB))
//				treeView.doubleClickCB(elem.userData);
//		}
//		if((k==10) && ctrl && elem) //ctrl+enter
//		{
//			if(elem.sub.length>0)
//			{
//				TreeWidget_toggleCB(id);
//				TreeWidgetElem_UpdateTooltip(source);
//			}	
//				
//			if (elem.isIncomplete&&elem.querycompleteCB)
//			{
//				elem.querycompleteCB()
//				TreeWidgetElem_UpdateTooltip(source);
//			}
//				
//			return false
//		}		
//	}
//	else //no selection in tree view
//	{
//		//var source =_ie?_curWin.event.srcElement.id:e.target.id
//		var source =TreeIdToIdx(_ie?_curWin.event.srcElement:e.target)
//		var k=eventGetKey(e);		
//		if( k==13 )
//		{						
//			//TreeWidget_clickCB(source.slice(8+_codeWinName.length),false,null);
//			TreeWidget_clickCB(source,false,null);
//			TreeWidgetElem_UpdateTooltip(source,true);			
//		}
//	}
//	return true;
//}

// ================================================================================
//t=0
//function TreeWidget_keyDownCB(lay,e)
//// Internal key down  event callback
//// Internal Note: Do not forget to fix a bug in single and multi selection functions.
//{
//	
//
//	//multi selection treeview
//	if(getWidget(lay).multiSelection)
//	{ 
//		return TreeWidget_multiSelKeyDown(lay,e);		
//	}	
//	
//	//mono selection treeview
//	var id=getWidget(lay).selId;
//	var k=eventGetKey(e);
//	if (id>=0)
//	{
//		var elem=_TreeWidgetElemInstances[id]
//		if (elem!=null)
//		{	
//			var treeView=elem.treeView
//			//var source=_ie?_curWin.event.srcElement.id:e.target.id;
//			var source=TreeIdToIdx(_ie?_curWin.event.srcElement:e.target)
//			switch(k)
//			{
//				case 107:
//				case 39:
//					if ((elem.sub.length>0)&&(!elem.expanded))
//					{
//						TreeWidget_toggleCB(id);
//						TreeWidgetElem_UpdateTooltip(source);
//					}
//
//					if (elem.isIncomplete&&elem.querycompleteCB)
//					{
//						elem.querycompleteCB()
//						TreeWidgetElem_UpdateTooltip(source);
//					}
//
//					break;
//				case 109:
//				case 37:
//					if ((elem.sub.length>0)&&(elem.expanded))
//					{
//						TreeWidget_toggleCB(id);
//						TreeWidgetElem_UpdateTooltip(source);
//					}
//					break;
//				case 40:
//				case 38:
//					var nElt=elem.getNextPrev(k==40?1:-1);if (nElt!=null){nElt.select(null,null,null,true);safeSetFocus(nElt.domElem)} 
//					return false
//					break;
//				case 46: //remove				
//					if(treeView.deleteCB)	
//						treeView.deleteCB(elem.userData)
//					break;	
//				default:
//					//key is alpha numerique, select the first occurence that starts with this character from the current position
//					var c = String.fromCharCode(k);					
//					if(c)//text,matchCase,matchWholeW,startFrom,next,notFoundCB,starWith,visible,setFocus)
//					{						
//						treeView.search(c,false,false,null,true,null,true,true,true);
//					}
//				break	
//			}
//		}
//	}
//	//To implement default action in dialog box with the key Enter
//	//we should cancel the bubble for treeview because Enter should not execute 
//	//the default action in dialog box when it is done on a treview item.
//	if(k == 13)
//	{
//		eventCancelBubble(e);
//	}
//}
//// ================================================================================
//function TreeWidget_multiSelKeyDown(o,e)
//{
//	//multi selection treeview
//	var treeView = getWidget(o);
//	var len = treeView.selIds.length;
//	var k=eventGetKey(e);
//	if (len>0) //already has selection
//	{				
//		var ctrl=_ie?_curWin.event.ctrlKey:e.ctrlKey;
//		var shift=_ie?_curWin.event.shiftKey:e.shiftKey;
//		//var source=_ie?_curWin.event.srcElement.id:e.target.id;
//		var source=TreeIdToIdx(_ie?_curWin.event.srcElement:e.target)
//			
//		//find the source in the selected items
//		var elem = null, id;
//		
//		for(var i=0; i<len;i++)
//		{
//			id = treeView.selIds[i];
//			//if (source==_codeWinName+"trLstElt"+id)
//			if (source==id)
//			{
//				elem = _TreeWidgetElemInstances[id];
//				break;
//			}
//		}
//		if(elem)
//		{
//			switch(k)
//			{
//				case 107: //+ ou ->
//				case 39:
//					//no change for multi selection 
//					if ((elem.sub.length>0)&&(!elem.expanded))
//					{
//						TreeWidget_toggleCB(id);
//						TreeWidgetElem_UpdateTooltip(source);
//					}
//
//					if (elem.isIncomplete&&elem.querycompleteCB)
//					{
//						elem.querycompleteCB()
//						TreeWidgetElem_UpdateTooltip(source);
//					}
//
//					break;
//				case 109: //- ou <-
//				case 37:
//					//no change for multi selection 
//					if ((elem.sub.length>0)&&(elem.expanded))
//					{
//						TreeWidget_toggleCB(id);
//						TreeWidgetElem_UpdateTooltip(source);
//					}
//					break;
//				case 40: //up or down arrow				
//				case 38:
//					//var nElt=elem.getNextPrev(k==40?1:-1);if (nElt!=null){nElt.select(null,null,null,true);safeSetFocus(nElt.domElem)} return false;
//					var nElt=elem.getNextPrev(k==40?1:-1);
//					if (nElt!=null)
//					{
//						nElt.select(null,_ie?_curWin.event:e,null,true);
//						safeSetFocus(nElt.domElem)
//					} 
//					return false
//					break;
//				case 46: //remove		
//					//IMPORTANT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//					//deleteCB will only send userData of the elem where the action is performed
//					//code must be added by the user in the deleteCB to manage the action for all the selected items
//					//please refer to sample treeview.html for more details		
//					if(treeView.deleteCB)	
//						treeView.deleteCB(elem.userData)
//					break;	
//					
//				default:
//					//key is alpha numerique, select the first occurence that starts with this character from the current position
//					var c = String.fromCharCode(k);					
//					if(c)//text,matchCase,matchWholeW,startFrom,next,notFoundCB,starWith,visible,setFocus)
//					{						
//						treeView.search(c,false,false,null,true,null,true,true,true);
//					}
//				break			
//			}
//		}		
//	}
//	//To implement default action in dialog box with the key Enter
//	//we should cancel the bubble for treeview because Enter should not execute 
//	//the default action in dialog box when it is done on a treview item.
//	if(k == 13)
//	{
//		eventCancelBubble(e);
//	}
//}
// ================================================================================

function TreeWidget_setDragDrop(dragCB,acceptDropCB,dropCB,dragEndCB)
// Add drag & drop behaviour to the tree view
// dragCB       (source)                       : triggered when beginning D&D
// acceptDropCB (source, target, ctrl, shift)  : return boolean if D&D is accepted
// dropCB       (source, target,ctrl, shift)   : triggered when ending D&D
{
	this.dragCB=dragCB
	this.acceptDropCB=acceptDropCB
	this.dropCB=dropCB
	this.dragEndCB=dragEndCB

	this.dragDrop=newDragDropData(this,TreeWidget_dragStartCB,TreeWidget_dragCB,TreeWidget_dragEndCB,TreeWidget_acceptDropCB,TreeWidget_leaveDropCB,TreeWidget_dropCB)
}

// ================================================================================

function TreeWidget_dragStartCB(src)
// Internal
// src : widget that started the drag&drop action
{
	//var item=src.getSelectedItem(),vert=src.iconOrientVertical
	var items=src.getSelections(),vert=src.iconOrientVertical

	src.dragCB(src)

	if (items && items.length==1 )
	{
		var item=items[0]; //display the tooltip of one selection
		var idx=item.iconId
		newTooltipWidget().show
		(
			true,
			item.getDragTooltip(),//item.name,
			idx>=0?src.icns:null,
			src.iconW,
			src.iconH,
			vert?0:src.iconW*idx,
			vert?src.iconH*idx:0
		)		
	}
}

// ================================================================================
//this function must be called before the construction of HTML
function TreeWidget_setRightClickMenuCB(rightClickMenuCB)
{
	this.rightClickMenuCB=rightClickMenuCB
}

// ================================================================================


// ================================================================================
function TreeWidget_getCount()     
{
	var o=this
	if (o.sub != null)
		return o.sub.length
	else
		return 0;
}
// ================================================================================
//this function must be called before the construction of HTML
function TreeWidget_setTooltipOnMouseOver(catchMouseOver)
{
	this.mouseOverTooltip=catchMouseOver
}

// ================================================================================
function TreeWidget_setMouseOverCB(mouseOverCB)
{
	this.mouseOverCB=mouseOverCB
}
// ================================================================================

function TreeWidget_dragCB(src)     
{
	newTooltipWidget().setPos();	
}

// ================================================================================

function TreeWidget_dragEndCB(src)  {
	newTooltipWidget().show(false);
	if (src.dragEndCB) src.dragEndCB()	
}

// added for inside tree D&D
function TreeWidget_dragOverEnterCB(lyr,elemId)
{

	var e=_TreeWidgetElemInstances[elemId]

	if (lyr.ondrop==null)	// layer
	{
		e.treeView.dragDrop.attachCallbacks(lyr,true)
		lyr.domEltID=elemId
	}

	var o=_ddData[lyr._dragDropData],e=_curWin.event
	e.dataTransfer.dropEffect=e.ctrlKey?'copy':'move'	
	
	if (o.acceptDropCB(window._globalDDD,o.widget,e.ctrlKey,e.ctrlKey?false:e.shiftKey,lyr,false))
		e.returnValue=false

	e.cancelBubble=true
}

// ================================================================================

function TreeWidget_acceptDropCB(src,target,ctrl,shift,layer) 
{									
	return target.acceptDropCB(src,target,ctrl,shift,layer)// the callback defined by the client
}

// ================================================================================

function TreeWidget_leaveDropCB(src,target,ctrl,shift) 
{
	
	if (target.dropWidget && target.dropWidget.layer) {
		//window.status="leave: " + target.dropWidget.layer.id
		if (target.dropWidget.layer.className != target.dropWidget.nonselectedClass) {
			target.dropWidget.layer.className = target.dropWidget.nonselectedClass
		}	
		//target.dropWidget = null
	}
	
}

// ================================================================================

function TreeWidget_dropCB(src,target,ctrl,shift,layer,enter)       
{
	newTooltipWidget().show(false);
	
	// added for inside tree D&D
	//target.dropWidget=_TreeWidgetElemInstances[layer.domEltID]
		
	//target.dropWidget.layer.className = target.dropWidget.nonselectedClass
	//	
	target.dropCB(src,target,ctrl,shift);
}

// ================================================================================
//set multi selection in treeview
function TreeWidget_setMultiSelection(multi)
{
	//clean the selection structure before start new selection mode
	if((!this.multiSelection && multi)||(this.multiSelection && !multi))
		this.unselect();
	
	//set selection mode 	
	this.multiSelection = multi;	
}
// ================================================================================
function TreeWidget_getSelectedItems()
// deprecated use getSelections()
// Get the selected items
// Returns  an array of [TreeWidgetElem] selected (empty if no selection)
{
	var arrSel=new Array;
	var len = this.selIds.length, id, cpt=0;
	for(var i=0; i< len; i++)
	{
		id = this.selIds[i];
		if(id>=0)
		{
			arrSel[cpt]=_TreeWidgetElemInstances[id];	
			cpt++;
		}
		//arrSel[i]= (id>=0)?_TreeWidgetElemInstances[id]:null;
	}
	return arrSel;
}

// ================================================================================
function TreeWidget_getCheckedItems()
// Get the checked items
// Returns  an array of [TreeWidgetElem] selected (empty if no selection)
{
	var arrChecked=new Array;
	var len = _TreeWidgetElemInstances.length, cpt=0;
	for (var i=0; i < len; i++)
	{
		elem = _TreeWidgetElemInstances[i]		
		if (elem.isChecked())
		{
			arrChecked[cpt]=elem;	
			cpt++;
		}		
	}
	return arrChecked;
}

// ================================================================================
//set highlight path parameter in treeview
function TreeWidget_setHighlightPath(hl)
{
	this.hlPath=hl;
	if(!hl)
		this.unhlPath();
}
// ================================================================================
function TreeWidget_unhlPath()
{
	var o=this, len = o.hlElems.length;
	var elem, de;
	if(len>0)//unhighlight previous elems
	{
		for(var i=0;i<len;i++)
		{			
			elem = o.hlElems[i];
			elem.init();
			de =elem.domElem;
			
			if( de == null) return;//safe test
			
			if(elem.isSelected())
				de.className=elem.selectedClass;
			else	
				de.className=elem.nonselectedClass;
		}
		o.hlElems.length=0;
	}
}
//highlight path of the selected item
function TreeWidget_highlightPath(elemId)
{
	var o=this;
	
	if(!o.hlPath) return ; //safe test
	
	o.unhlPath();
	
	var elem = _TreeWidgetElemInstances[elemId];	
	
	//highlight and select item
	o.hlElems[o.hlElems.length]=elem;
	elem.domElem.className=elem.selectedClass;
	
	//highlight parents	
	var papa = elem.par;
	while(papa)
	{
		papa.init();		
		papa.domElem.className=papa.hlClass;
		o.hlElems[o.hlElems.length]=papa;	
		papa = papa.par;
	}
	
	//highlight children
	if(elem.isNode())	
		hlVisibleChildren(elem,o.hlElems);		
}

function hlVisibleChildren(node,arr)
{
	if(node.expanded && !node.isIncomplete)
	{
		var len = node.sub.length;
		for(var i=0;i<len;i++)
		{
			var sub = node.sub[i];
			arr[arr.length]=sub;
			sub.init();
			sub.domElem.className=sub.hlClass;
			if(sub.isNode()) 							
				hlVisibleChildren(sub,arr);			
		}
	}
}

function TreeWidget_getFirst()
{
	var o=this;
	if (o.sub && o.sub.length > 0) {
		return o.sub[0];
	}
	return null;
}

function TreeWidget_getLast()
{
	var o=this;
	var sub=o.sub;
	while (sub && sub.length > 0) {
		// get the last node in the sub array
		var last = sub[sub.length - 1];
		if (last.expanded && last.sub && last.sub.length > 0) {
			sub = last.sub;
		}
		else {
			return last;
		}
	}
	return null;
}

function TreeWidget_focusCB(tree)
{
	var o = getWidget(tree);
	if (o && o.focusNode) {
		o.setFocus(o.focusNode.id);
	}
}

// ================================================================================
// ================================================================================
//
// OBJECT newTreeWidgetElem (Constructor)
//
// Tree view element class
//
// ================================================================================
// ================================================================================

function newTreeWidgetElem(iconId,name,userData,help,iconSelId,tooltip,iconAlt,textClass,textSelectedClass, enableDoubleClick, renderRTLHint)
// CONSTRUCTOR
// iconId        [int] the index of the icon in the combined image, -1 if no icon
// name          [String] name of the elem
// userData      [Optional]user data
// help          [Optional]help of the elem (tooltip ???)
// iconSelId	 [int Optional] the index of the icon in the combined image when elem is selected
// tooltip       [Optional]tooltip of the elem
// iconAlt       [Optional]tooltip of icon 
// Return     [newTreeWidgetElem] the instance

// to not encode the elemen content, set isHTML to true
{
	var o=new Object

	// Data
	o.enableDoubleClick = enableDoubleClick;
	o.expanded=false
	o.generated=false
	o.iconId=iconId
	o.iconSelId=iconSelId?iconSelId:iconId
	o.tooltip=tooltip
	o.customTooltip=false
	o.iconAlt=iconAlt
	o.isHTML=false
	o.renderRTLHint= renderRTLHint
	
				
	// Check box
	o.isCheck=false
	o.checked=false
	o.check=TreeWidgetElem_check
	o.isChecked=TreeWidgetElem_isChecked
	o.checkCB=null
	
	o.name=name
	if(hasNoRTLCharacters(name))
		o.name += "\u200e"
	
	o.par=null
	o.userData=userData
	o.sub=new Array
	o.treeView=null
	o.id=_TreeWidgetElemInstances.length
	o.index=-1

	// Layers
	o.layer=null
	o.plusLyr=null
	o.icnLyr=null
	o.checkElem=null
	o.domElem=null
	o.toggleLyr=null
	o.actualNumChildren=null;

	o.blackTxt=(textClass)?textClass:'treeNormal'
	o.grayTxt='treeGray'

	o.selectedClass=(textSelectedClass)?textSelectedClass:'treeSelected'	
	o.nonselectedClass=o.blackTxt
	o.feedbackDDClass='treeFeedbackDD'
	
	o.hlClass='treeHL' //hl=highlight	
		
	o.cursorClass=null
		
	o.help=help

	_TreeWidgetElemInstances[o.id]=o

	// Methods
	o.getHTML=TreeWidgetElem_getHTML
	o.init=TreeWidgetElem_init
	o.add=TreeWidgetElem_add;
	o.select=TreeWidgetElem_select
	o.unselect=TreeWidgetElem_unselect
	o.getNextPrev=TreeWidgetElem_getNextPrev
	o.getHiddenParent=TreeWidgetElem_getHiddenParent
	o.nodeIndent=0
	o.getTooltip=TreeWidgetElem_getTooltip
	o.getDragTooltip=TreeWidgetElem_getDragTooltip
//	o.change=TreeWidgetElem_change
	o.deleteAll=TreeWidget_deleteAll
	
	o.setGrayStyle=TreeWidgetElem_setGrayStyle
	o.isGrayStyle=TreeWidgetElem_isGrayStyle

	o.findByData=TreeWidgetElem_findByData
	o.findById=TreeWidgetElem_findById
//	o.findInName=TreeWidgetElem_findInName

	o.isIncomplete=false
	o.querycompleteCB=null
	o.setIncomplete=TreeWidgetElem_setIncomplete
	o.finishComplete=TreeWidgetElem_finishComplete
	
	o.setEditable=TreeWidgetElem_setEditable
//	o.showEditInput=TreeWidgetElem_showEditInput
	
	o.isLeaf=TreeWidgetElem_isLeaf
	o.isNode=TreeWidgetElem_isNode
	o.isSelected=TreeWidgetElem_isSelected

	o.htmlWritten=false
	
	o.showCustomTooltipCB=null
	o.hideCustomTooltipCB=null
	
	o.setCursorClass=TreeWidgetElem_setCursorClass
	
	return o
}

// ================================================================================

function TreeWidgetElem_checkCB(elem, id)
// PRIVATE
{
	var o=_TreeWidgetElemInstances[id]
	o.checked=elem.checked
	
	if (o.checkCB)
		o.checkCB(o, id)
}

//================================================================================

function TreeWidgetElem_iconFocusCB(elem)
{
	while (elem && elem.nextSibling) {
		elem = elem.nextSibling;
	}
	
	if (elem)
		elem.focus();
}

// ================================================================================

function TreeWidgetElem_isChecked()
// Test is the element is checked (works if the TreeWidgetElem is a checkbox)
// Return boolean
{
	var o=this
	return (o.isCheck ? o.checked : false)
}

// ================================================================================

function TreeWidgetElem_check(checked)
// check or uncheck the element (works if the TreeWidgetElem is a checkbox)
// checked [boolean - Mandatory]
{
	var o=this
	if (o.isCheck)
	{
		o.checked = checked
		
		if (o.htmlWritten)
		{
			o.init()
			o.checkElem.checked = checked
		}
	}
}

// ================================================================================

function TreeWidgetElem_EditNormalBehaviour(e)
// PRIVATE
{
	eventCancelBubble(e)
	return true
}

// ================================================================================

function TreeWidgetElem_EditBlurCB()
// PRIVATE
{
	var widID = this.widID;
	setTimeout( function() {
		TreeWidgetElem_EditKeyCancel(widID)
	}, 1)

}

// ================================================================================

function TreeWidgetElem_EditKeyDown(e)
// PRIVATE
{
	eventCancelBubble(e); //do not fire the event in TreeWidgetElem
	
	var k=eventGetKey(e),o=_TreeWidgetElemInstances[this.widID], widID = this.widID;
	                                                
	if (k==27) // Escape
	{
	    setTimeout( function() {
	        TreeWidgetElem_EditKeyCancel(widID)
	    }, 1)
	}
	else if (k==13) // Enter
	{		
		var newValue = this.value;
		setTimeout( function() {
			TreeWidgetElem_EditKeyAccept(widID, newValue)
		}, 1)	
	}
}

// ================================================================================

function TreeWidgetElem_EditKeyCancel(id)
// PRIVATE
{
	var o=_TreeWidgetElemInstances[id]
	o.showEditInput(false)
}


// ================================================================================

function TreeWidgetElem_EditKeyAccept(id, newValue)
// PRIVATE
{
	var o=_TreeWidgetElemInstances[id]

	if (o.validChangeNameCB)
	{
		if (o.validChangeNameCB(newValue)==false)
			return
	}
		
	o.change(null, newValue)
	o.showEditInput(false)

	if (o.changeNameCB)
		o.changeNameCB()
}

// ================================================================================

/*
_globTreeTxt=null
function TreeWidgetElem_showEditInput(show)
{
	var o=this
	
	o.init()
	
	var lyr=o.domElem,css=lyr.style
	
	if (show&&(css.display!="none"))
	{
		var par=lyr.parentNode,w=lyr.offsetWidth,h=lyr.offsetHeight
		css.display="none"
		
		var tl=_globTreeTxt=_curDoc.createElement("INPUT");
		tl.type="text"
		tl.className="textinputs"
		tl.value=o.name
		tl.ondragstart=TreeWidgetElem_EditNormalBehaviour
		tl.onselectstart=TreeWidgetElem_EditNormalBehaviour
		tl.onblur=TreeWidgetElem_EditBlurCB
		tl.onkeydown=TreeWidgetElem_EditKeyDown
		
		tl.widID=o.id
		
		var tc=tl.style
		tc.width=""+(w+20)+"px"
		
		par.appendChild(tl);
		tl.focus()
		tl.select()
	}

	if ((show!=true)&&(css.display=="none"))
	{
		var tl=_globTreeTxt
		if (tl)
		{
			tl.parentNode.removeChild(tl)
			css.display=""
			_globTreeTxt=null
		}
	}
}
*/

// ================================================================================

function TreeWidgetElem_setEditable(isEditable, changeNameCB,validChangeNameCB)
// Set the elem editable (i.e. the name can be changed)
// changeNameCB [function optional]: callback when the name is changed
// validChangeNameCB [function optional] : callback called before text is chaged. Returns a boolean
// if true, the changes are accepted
{
	var o=this
	
	if (isEditable)
	{
		o.changeNameCB=changeNameCB
		o.validChangeNameCB=validChangeNameCB
	}
	o.isEditable=isEditable
}

// ================================================================================

function TreeWidgetElem_triggerDD()
{
	var o=_treeWClickedW,e=_curWin.event

	if (o&&(o.clicked)&&(e.button==_leftBtn))
	{
		if (o.initialX!=null)
		{
			var x=eventGetX(e),y=eventGetY(e),threshold=3
			
			if ((x<(o.initialX-threshold))||(x>(o.initialX+threshold))||(y<(o.initialY-threshold))||(y>(o.initialY+threshold)))
			{
				this.dragDrop()
				o.clicked=false
			}
		}
	}
}


// ================================================================================

function TreeWidgetElem_mouseUp()
{
	var o=_treeWClickedW,ev=_curWin.event
	
	o.select(null,ev)
	o.domElem.onmouseup=null
}

// ================================================================================
// pass in the widget's layer to reduce calls to document.getElementById as it's very expensive
function TreeWidgetElem_init(layer)
{
	var o=this
	if (o.layer==null)
	{
		var sub=o.sub,len=sub.length,exp=(len>0)||o.isIncomplete

		// Init Widgets			
		o.layer = layer ? layer : getLayer(_codeWinName+"TWe_"+o.id);

		if (o.layer==null)
			return;

		var cNodes=o.layer.childNodes,cLen=cNodes.length
		
		o.plusLyr=exp?cNodes[0]:null
		o.icnLyr=(o.iconId>-1)?cNodes[exp?1:0]:null
		o.checkElem=o.isCheck?cNodes[cLen-2]:null
		o.domElem=cNodes[cLen-1]
		                   
		if(o.layer.nextSibling && o.layer.nextSibling.id == _codeWinName+"trTog"+o.id)
			o.toggleLyr=o.layer.nextSibling;
		
		// Init Callbacks
		if(o.treeView.mouseOverTooltip||o.treeView.mouseOverCB)
			o.domElem.onmouseout=TreeFuncMouseout

		if (exp)
		{
			addDblClickCB(o.plusLyr,_tpdb)
		}
		
		if (exp&&o.generated)
		{
			for (var i = 0; i < sub.length; i++)
				sub[i].init()
		}
		
		if(o.enableDoubleClick) 
			addDblClickCB(o.domElem,_tpdb)
	}
}

// ================================================================================

function TreeIdToIdx(l)
{
	if (l)
	{
		var id=l.id
		if (id)
		{
			var idx=id.lastIndexOf("TWe_")
			if (idx>=0)
				return parseInt(id.slice(idx+4))
			else
				return -1
		}
		else
			return TreeIdToIdx(l.parentNode)
	}
	return -1
}

function TreeFuncMouseout(e)
{
	_tmoc(this,TreeIdToIdx(this),false,e)
}

function _tmvc(l,ev)
{
	_tmoc(l,TreeIdToIdx(l),true,ev)
}

function _tpl(l,event)
// PRIVATE
// Single click on the +/- icon
{
	TreeWidget_clickCB(TreeIdToIdx(l),true,event,true);
	return false
}

function _tkt(l,event)
// PRIVATE
// Enter key on text
{
	var k = eventGetKey(event);
	var stopPropagation = false;
	switch (k)
	{
	case KEY_ENTER:
	case KEY_SPACE:
		return _tpt(l,event);
	case KEY_LEFT:
	case KEY_RIGHT:
		stopPropagation = true;
		TreeWidget_clickCB(TreeIdToIdx(l),true,event,true,k==KEY_RIGHT);
		break;
	case KEY_END:
	case KEY_HOME:
		stopPropagation = true;
		break;
	case KEY_PAGEUP:
	case KEY_PAGEDOWN:
	case KEY_UP:
	case KEY_DOWN:
		stopPropagation = true;
		TreeWidget_keydownCB(TreeIdToIdx(l), event, k);
		break;
	default:
		break;
	}
	
	if (stopPropagation) {
		eventCancelBubble(event);
		eventPreventDefault(event);
	}
}

function _tpt(l,event)
// PRIVATE
// Single click on text
{
	TreeWidget_clickCB(TreeIdToIdx(l),false,event,true);
	return false
}

function _tpdb(e)
// PRIVATE
// Double click on the +/- icon
{
	treeDblClickCB(TreeIdToIdx(this),_ie?event:e)
	return false
}

// ================================================================================

function TreeWidgetElem_getHTML(indent,isFirst)
// Get the tree element HTML
// indent [int] : element indentation in pixels
// returns [String] the HTML
{
	var s='';

	with (this)
	{
		htmlWritten=true
		var isRoot=(par==null)
		var len=sub.length,exp=(len>0)||isIncomplete,a=new Array,i=0

		if (this.extraIndent)
		    indent+=_trIndent*extraIndent

		var keyCB = 'onkeydown=" return ' + _dhtmlLib+'._tkt(this,event)" ';
		var mouseCB='onclick="return '+_dhtmlLib+'._tpt(this,event)" '		
		if(treeView.mouseOverTooltip || treeView.mouseOverCB)
			mouseCB+='onmouseover="'+_dhtmlLib+'._tmvc(this,event)" '
				
		var contextMenu=''
		if (treeView.rightClickMenuCB != null)
		{
			contextMenu= ' oncontextmenu="' + _dhtmlLib + '.treeContextMenuCB(\''+ id + '\', event);return false" '
		}
		
		
		var acceptDD=''					
		if ((treeView.acceptDropCB != null) && (_ie))
		{
			acceptDD= ' ondragenter="' + _dhtmlLib + '.TreeWidget_dragOverEnterCB(this,\''+id+'\');" '
			acceptDD += ' ondragover="' + _dhtmlLib + '.TreeWidget_dragOverEnterCB(this,\''+id+'\');" '
		}	
		

		// begin container
		a[i++]='<div id="'+_codeWinName+'TWe_'+id+'"'+contextMenu+' class="trElt" role="presentation">'
		var onclick='onclick="return '+_dhtmlLib+'._tpl(this,event)"';
		            
		if (exp)
		{
		    var expIcon;
		    if(expanded)
		    {
		        if(treeView.minIcon != null)
		            expIcon = treeView.minIcon;
		        else
		            expIcon = _skin+'../min.gif';
		    }
		    else
		    {
		        if(treeView.plusIcon != null)
		            expIcon = treeView.plusIcon;
		        else
		            expIcon = _skin+'../plus.gif';
		    }
	        
		    a[i++]='<img tabindex="-1" '+onclick+' role="presentation" class=trPlus src="'+expIcon+'" onfocus="'+_dhtmlLib+'.TreeWidgetElem_iconFocusCB(this)" style="cursor:'+_hand+'"/>'
		}
			
		if (iconId>-1)
		{
			var iconClass='trIcn'+(exp||isRoot?'Plus':'')
			if (this.cursorClass)
				iconClass+=' '+this.cursorClass
			a[i++]='<img tabindex=-1 '+mouseCB + ' ' + keyCB +'class="'+iconClass+'" '+attr('src', _skin+'../transp.gif')+attr('alt',iconAlt)+' role="presentation" align="top" style="'+backImgOffset(treeView.icns,(treeView.iconOrientVertical?0:treeView.iconW*(expanded?iconSelId:iconId)),(treeView.iconOrientVertical?treeView.iconH*(expanded?iconSelId:iconId):0))+'" >'
		}
		else if (!exp&&!isRoot)
			a[i++]='<img tabindex=-1 class=trSep '+attr('src', _skin+'../transp.gif')+'>'

		if (isCheck)
			a[i++]='<input type=checkbox style="margin:0px;" onclick="'+_dhtmlLib+'.TreeWidgetElem_checkCB(this,\''+id+'\')"'+(this.checked?' checked':'')+'>'
		
		var textClass=nonselectedClass
		if (this.cursorClass)
			textClass+=' '+this.cursorClass
		
		var setSize = 0;
		if (par) {
			if (par.actualNumChildren)
				setSize = par.actualNumChildren;
			else if (par.sub && par.sub.length > 0)
				setSize = par.sub.length;
		}
		else if (treeView.sub && treeView.sub.length > 0){
		    setSize = treeView.sub.length;
		}
		var aria = (exp?'aria-expanded="'+expanded+'"':'')+
		           (getLevel?'aria-level="'+getLevel()+'"':'')+
		           (index>=0?'aria-posinset="'+(index+1)+'"':'')+
		           'aria-setsize="'+setSize+'"';
		           
		a[i++]='<a href="javascript:doNothing();" '+mouseCB+ ' ' + keyCB + ' tabindex='+(isFirst?'0':'-1')+acceptDD+' class="'+textClass+'" '+aria+' role="treeitem" dir="' + (renderRTLHint?'rtl':'ltr') + '" >'
		a[i++]=(isHTML?name:convStr(name))
		a[i++]='</a>'
		a[i++]='</div>'
		
		
		
		// end container
		
		
		/*
		a[i++]='<nobr>'
		
		// Icon
		a[i++]='<span id="'+_codeWinName+'icn'+id+'" '+(_moz?'onclick':'onmousedown')+'="'+_codeWinName+'.TreeWidget_clickCB('+id+',true,event,true); if (_ie) return false" ondblclick="'+_codeWinName+'.treeDblClickCB('+id+',event);return false" ' + contextMenu + '>'
		a[i++]=(exp?imgOffset(_skin+'../tree.gif',13,12,expanded?0:13,0,null,null,expanded?L_DHTMLLIB_expandedLab:L_DHTMLLIB_collapsedLab,mrg,'top'):'')
		a[i++]=(iconId>-1?imgOffset(treeView.icns,treeView.iconW,treeView.iconH,(treeView.iconOrientVertical?0:treeView.iconW*(expanded?iconSelId:iconId)),(treeView.iconOrientVertical?treeView.iconH*(expanded?iconSelId:iconId):0),null,null,,exp?'':mrg,'top'):'')
		a[i++]='</span>'


		// Text		
		a[i++]=((!exp && iconId==-1)?img(_skin+'../transp.gif',indent+13,1)  :'')
				
		// Check box related
		if (isCheck)
		{
			a[i++]='<input onclick="'+_codeWinName+'.TreeWidgetElem_checkCB(this,\''+id+'\')" style="margin:0px;'+(_ie?'':'margin-left:2px')+'" type="checkbox" id="chk'+id+ '"' +(this.checked?' checked':'')+'>'
		}

		a[i++]='<a style="height:'+(iconId>-1?16:14)+'px;padding-top:0px;padding-bottom:0px;" 
			id="'+_codeWinName+'trLstElt'+id+'" class="'+nonselectedClass+ '" ' + acceptDD + ' href="javascript:void(0)" 
			onclick="eventCancelBubble(event);return false" onmousedown="'+_codeWinName+'.TreeWidget_clickCB('+id+',false,event,true);return false" 
			ondblclick="'+_codeWinName+'.treeDblClickCB('+id+',event);return false" 
			onfocus="'+_codeWinName+'.treeFCCB(this,'+id+',true)" 
			onblur="'+_codeWinName+'.treeFCCB(this,'+id+',false)" '+ mouseCB + contextMenu + '>'
		a[i++]=(isHTML?name:convStr(name))
		a[i++]='</a>'

		// End container
		a[i++]='</nobr><br>'
		*/


		// Sub tree container
		//if (exp) a[i++]='<span id="'+_codeWinName+'trTog'+id+'" style="display:'+(expanded?'':'none')+'">'
		
		if (exp)
		{
			a[i++]='<div id="'+_codeWinName+'trTog'+id+'" style="' + (_rtl?"margin-right":"margin-left")+':18px;display:'+(expanded?'':'none')+'" role="group">'
		}
		
		// Generate child HTML if needed
		if (expanded)
		{
			generated=true
			//var idt=indent+_trIndent
			//for (var j=0;j<len;j++) a[i++]=sub[j].getHTML(idt,j==0);
			for (var j=0; j < len; j++) 
			{
				a[i++] = sub[j].getHTML(0);
			}
		}

		if (exp)
		{
			nodeIndent=indent
			a[i++]="</div>"
		}
	}

	return a.join("");
}


// ================================================================================

function TreeWidgetElem_setGrayStyle(isGray)
{

	var o=this,cls=isGray?o.grayTxt:o.blackTxt

	if (cls!=o.nonselectedClass)
	{
		o.nonselectedClass=cls
		
		o.init()
		if (o.domElem&&(o.domElem.className!=o.selectedClass))
			o.domElem.className=cls
	}
}

// ================================================================================

function TreeWidgetElem_isGrayStyle()
{
	return this.nonselectedClass==this.grayTxt
}

// ================================================================================

function TreeWidgetElem_setIncomplete(querycompleteCB)
{
	this.isIncomplete=true
	this.querycompleteCB=querycompleteCB
}

// ================================================================================

function TreeWidgetElem_finishComplete()
{
	this.isIncomplete=false
	TreeWidget_toggleCB(this.id)
}

// ================================================================================

function TreeWidgetElem_findByData(data)
{
	var o=this
	if (o.userData==data)
		return o
	var sub=o.sub
	for (var i = 0; i < sub.length; i++)
	{
		var item=sub[i].findByData(data)

		if (item!=null)
		    return item
	}
	return null
}

// ================================================================================

//function TreeWidgetElem_findInName(text,matchCase,matchWholeW,next,starWith)
//{
//	var o=this, name=o.name
//	
//	if(text=="" || text==null) return; //safe test
//	
//	if(!matchCase || (matchCase == null))//ignore case
//	{
//		name=name.toLowerCase();
//		text=text.toLowerCase();
//	}	
//	if(matchWholeW)//match word
//	{
//		var arrWords = name.split(" ");//to improve later
//		for(var i = 0; i<arrWords.length; i++)
//		{
//			if(arrWords[i] == text)
//				return o;
//		}		
//	}
//	else
//	{
//		var idx = name.indexOf(text); // search occurence
//		if (starWith == true ) 
//		{
//			if(idx == 0) return o;
//		}
//		else
//		{
//			if(idx>-1) return o;
//		}			
//	}
//		
//	return null
//}

// ================================================================================

function TreeWidgetElem_findById(id)
{
	var o=this
	if (o.id==id)
		return o
	var sub=o.sub
	for (var i = 0; i < sub.length; i++)
	{
		var item=sub[i].findById(id)

		if (item!=null)
		    return item
	}
	return null
}
// ================================================================================

//function TreeWidgetElem_change(iconId, name, userData, help,iconSelId,tooltip)
//{
//	var o=this,treeView=o.treeView
//	if (iconId!=null) o.iconId=iconId
//	if (name!=null) o.name=name
//	o.userData=userData
//	if (help!=null) o.help=help
//	o.iconSelId=(iconSelId!=null)?iconSelId:o.iconId
//	if (tooltip!=null) o.tooltip=tooltip
//
//	o.init()
//
//	if (o.domElem)
//		o.domElem.innerHTML=convStr(o.name)
//
//	if (o.icnLyr)
//	{
//		if(o.icnLyr.childNodes.length>0)
//		{
//			var iconL=o.icnLyr.childNodes[o.sub.length>0?1:0]		
//			changeOffset(iconL,
//		             treeView.iconOrientVertical?0:o.treeView.iconW*(o.expanded?o.iconSelId:o.iconId),
//		             treeView.iconOrientVertical?o.treeView.iconH*(o.expanded?o.iconSelId:o.iconId):0)
//		}
//	}
//}

// ================================================================================

// added for inside tree D&D
function treeInitDropFunc(lyr,elemId)
{
	var e=_TreeWidgetElemInstances[elemId]
	
	if (lyr.ondrop==null)					// lyr
	{
		e.treeView.dragDrop.attachCallbacks(lyr,true)
		lyr.domEltID=elemId
	}
}

// ================================================================================
function TreeWidget_toggleCB(elemId,noTimeOut)
// Expand or collapse a tree element children
// Returns [void]
{
	var elem=_TreeWidgetElemInstances[elemId]
	
	// if no children the + disappear
	if (elem.sub.length==0) {
		elem.plusLyr.style.visibility='hidden'
		return
	}
	elem.expanded=!elem.expanded
	elem.init()

	if (noTimeOut) {
		dispIcn(elemId);
	} else {
		setTimeout(function () {elem.treeView.dispIcnFuncName(elemId); },1)
	}
	
	var tree=elem.treeView
	if (elem.expanded&&tree.expandCB)
		tree.expandCB(elem.userData)
	if (!elem.expanded&&tree.collapseCB)
		tree.collapseCB(elem.userData)

}

// ================================================================================

function dispIcn(eId)
{
	var e=_TreeWidgetElemInstances[eId]
	with (e)
	{
		if (expanded&&!generated)
		{
			generated=true;
			var a=new Array,i=0,len=sub.length,newInd=nodeIndent+_trIndent
			for (var j=0;j<len;j++) a[i++]=sub[j].getHTML(newInd);
			toggleLyr.innerHTML=a.join('');
		}

		toggleLyr.style.display=expanded?'block':'none'
		if(expanded)
		{
			if(treeView.minIcon != null)
				plusLyr.src = treeView.minIcon;
			else
				plusLyr.src = _skin+'../min.gif';
		}
		else
		{
			if(treeView.plusIcon != null)
				plusLyr.src = treeView.plusIcon;
			else
				plusLyr.src = _skin+'../plus.gif';
		}

		domElem.setAttribute("aria-expanded", expanded);
        
		if(icnLyr&&icnLyr.childNodes&&icnLyr.childNodes.length>1)
		{
			var iconL=icnLyr.childNodes[1]
			changeOffset(iconL,
		             treeView.iconOrientVertical?0:treeView.iconW*(expanded?iconSelId:iconId),
		             treeView.iconOrientVertical?treeView.iconH*(expanded?iconSelId:iconId):0)
		}
	}
}

// ================================================================================

function TreeWidgetElem_add(elem)
{
	with (this)
	{
		elem.treeView=treeView;
		elem.par=this;
		elem.index=sub.length;
		sub[sub.length]=elem;		
	}
	return elem
}

// ================================================================================

function TreeWidgetElem_getHiddenParent()
{
	var par=this.par
	if (par==null) return null
	if (!par.expanded)
		return par
	return
		par.getHiddenParent()
}

// ================================================================================

function TreeWidgetElem_getNextPrev(delta)
{
	var o=this;
    
	// recursive case for next
	if (delta > 0) {
		// if current node is expanded, recurse on the first sub node
		if (o.expanded && o.sub && o.sub.length > 0) {
			return o.sub[0].getNextPrev(delta - 1);
		}
		else if (o.par) {
			// recurse on the next sibling of the parent node
			var index = o.index;
			var par = o.par;
			while (par) {
				if (par.sub && index + 1 < par.sub.length) {
					return par.sub[index + 1].getNextPrev(delta - 1);
				}
				index = par.index;
				par = par.par;
			}
		}
	}
	// recursive case for prev
	else if (delta < 0) {
		if (o.par) {
			// if the current node has a previous sibling, recurse on the last sub node
			if (o.par.sub && (o.index - 1 >= 0)) {
				var prev = o.par.sub[o.index - 1];
				while (prev.expanded && prev.sub && prev.sub.length > 0) {
					prev = prev.sub[prev.sub.length - 1];
				}
                
				return prev.getNextPrev(delta + 1);
			}
			// recurse on the current node's parent
			else {
				return o.par.getNextPrev(delta + 1);
			}
		}
	}

	// base case
	return o;
}

// ================================================================================

function TreeWidgetElem_scroll(elemLyr,treeLyr)
{
	var scrollH = Math.max(0,treeLyr.offsetHeight-20), scrollY = treeLyr.scrollTop
	var elPos = getPos(elemLyr,treeLyr)
	var y = elPos.offsetTop, h = elemLyr.offsetHeight
	
	if ((y-scrollY+h) > scrollH ) {
		treeLyr.scrollTop=y+h-scrollH
	}
	if ((y-scrollY) < 0) {
		treeLyr.scrollTop= y
	}			
}

// ================================================================================

function TreeWidgetElem_unselect()
{
	var o=this
	with(o)
	{
		init()
		if (domElem) {
			domElem.className=o.nonselectedClass
		}
		treeView.selId=-1
	
		if(treeView.multiSelection)
		{
			var idx = arrayFind(treeView,'selIds',id)
			if(idx>-1)
			{
				//update array selIds
				arrayRemove(treeView,'selIds',idx);	
				
				//update _BOselIds
				treeView.layer._BOselIds=""
				var len = treeView.selIds.length;
				for(var i=0;i<len;i++)
				{
					if(treeView.layer._BOselIds == "")
						treeView.layer._BOselIds=""+treeView.selIds[i];
					else
						treeView.layer._BOselIds+=";"+treeView.selIds[i];
				}				
			}				
		}
	}
}


// ================================================================================

function TreeWidgetElem_select(setFocus,ev,noSendClickCB,isFromKeybArrow)
{
	// If some parents are collapsed, expand them before
	var coll=new Array
	var par=this.par

	while (par)
	{
		if (!par.expanded)
			coll[coll.length]=par
		par=par.par
	}
	
	var cLen=coll.length
	for (var i=cLen-1;i>=0;i--)
	{
		TreeWidget_toggleCB(coll[i].id,true)//direct construction of the HTML of children
	}	
	if (cLen>0)
	{
	/*
		_tvw_param0=this
		_tvw_param1=setFocus
		_tvw_param2=ev
		_tvw_param3=noSendClickCB
		_tvw_param4=isFromKeybArrow
		setTimeout("_tvw_param0.select(_tvw_param1,_tvw_param2,_tvw_param3,_tvw_param4)",1)
		return;
		*/
		this.select(setFocus,ev,noSendClickCB,isFromKeybArrow);
	}		
	
	//multi selection treeview
	if(this.treeView.multiSelection)
	{	 
		TreeWidgetElem_multiSelect(this,setFocus,ev,noSendClickCB,isFromKeybArrow);
		return;
	}
	
	//mono selection treeview
	if (noSendClickCB==null)
		noSendClickCB=false
	with (this)
	{
		if (treeView.selId!=id)
		{
			if (treeView.selId>=0)
			{
				var prev=_TreeWidgetElemInstances[treeView.selId]
				prev.init()
				if (prev.domElem) {
					prev.domElem.className=prev.nonselectedClass
					prev.domElem.removeAttribute("aria-selected");
				}	
				//window.status=prev.nonselectedClass
			}

			treeView.selId=id;
			init()
			treeView.layer._BOselId=id

			var de=domElem
			if (de == null) return
			
			//highlight path
			if(treeView.hlPath)			
				treeView.highlightPath(id);			
			else {
				de.className=selectedClass
				de.setAttribute("aria-selected", true);
			}
				
			//window.status=prev.selectedClass
			if (setFocus) {
				treeView.setFocus(id)
			}
			
			TreeWidgetElem_scroll(de,treeView.layer)
			
			
			//if ((treeView.clickCB)&&(!noSendClickCB)) treeView.clickCB(userData)
			//window.status= "Normal Elem_select=" + id
		}
		if ((treeView.clickCB)&&(!noSendClickCB)) treeView.clickCB(userData,isFromKeybArrow!=null?isFromKeybArrow:false)
	}
	// stop propagating event to the other link
	/*if (ev)
		eventCancelBubble(ev)*/ //DOESN'T WORK WITH NETSCAPE
}
// ================================================================================
var _startShift=null;
//function TreeWidgetElem_multiSelect(o,setFocus,ev,noSendClickCB,isFromKeybArrow)
//{		
//	if (noSendClickCB==null)
//		noSendClickCB=false
//	with (o) // o instanceof TreeWidgetElem
//	{		
//		init();
//		var de=domElem
//		if (de == null) return
//		
//		//reset highlight items
//		if(treeView.hlPath) treeView.unhlPath();
//		
//		if(ev == null)
//		{
//			var idx = arrayFind(treeView,'selIds',id);			
//			if(idx == -1)//not yet selected
//			{		
//				//add item to selected item list
//				treeView.selIds[treeView.selIds.length]=id;				
//				if(treeView.layer._BOselIds == "")
//					treeView.layer._BOselIds=""+id;
//				else
//					treeView.layer._BOselIds+=";"+id;
//
//				de.className=selectedClass										
//			}
//			//need to reset here ?
//			_startShift=null;			
//		}
//		else //from event, use ctl and shift to define the action to do
//		{
//			var idx = arrayFind(treeView,'selIds',id);
//			var ctrl=_ie?_curWin.event.ctrlKey:ev.ctrlKey
//			var shift=_ie?_curWin.event.shiftKey:ev.shiftKey
//			var typeEvt=_ie?_curWin.event.type:ev.type
//			
//			if(ctrl && !shift) //select or deselect
//			{					
//				if(idx == -1)//select item
//				{				   
//					// don't select if there's a grayed item
//					//if (o.isGrayStyle()) return	
//					
//					treeView.selIds[treeView.selIds.length]=id;					
//					if(treeView.layer._BOselIds == "")
//						treeView.layer._BOselIds=""+id;
//					else
//						treeView.layer._BOselIds+=";"+id;
//								
//					de.className=selectedClass				
//				}
//				else //deselect item
//				{					
//					unselect();						
//				}			
//				_startShift= o;
//			}			
//			if(shift) //select block items
//			{	
//				var lastSelId=-1,lastSel=null;				
//				if(treeView.selIds.length>0)	
//				{	
//					lastSelId = treeView.selIds[treeView.selIds.length-1];
//					lastSel = _TreeWidgetElemInstances[lastSelId];
//					
//					if(_startShift == null)										
//						_startShift = lastSel;				
//					
//					if(!ctrl) //do not clear selection if we want to continue shift selection after a ctrl action
//						treeView.unselect();	
//												
//					TreeWidgetElem_multiSelectShift(_startShift.id,id);																													
//				}
//				else //select only this item				
//				{
//					treeView.unselect();
//					treeView.selIds[0]=id;					
//					treeView.layer._BOselIds=""+id;
//				
//					if(treeView.hlPath)					
//						treeView.highlightPath(id);					
//					else
//						de.className=selectedClass;
//					
//					_startShift=null;
//				}						
//			}
//			if(!ctrl && !shift) //simple click
//			{	
//				//be carefull, sometimes we want to drag and drop so don't deselect	treeview
//				var idx = arrayFind(treeView,'selIds',id);
//				if( _ie &&typeEvt=="mousedown" && idx>-1)
//				{					
//					window._treeWClickedW=o
//					de.onmouseup=TreeWidgetElem_mouseUp
//				}
//				else
//				{
//					treeView.unselect();
//					treeView.selIds[0]=id;					
//					treeView.layer._BOselIds=""+id;
//					
//					if(treeView.hlPath)				
//						treeView.highlightPath(id);				
//					else
//						de.className=selectedClass		
//					
//					_startShift=null;
//				}
//			}						
//		}
//		if (setFocus) {
//			safeSetFocus(de)
//		}	
//		TreeWidgetElem_scroll(de,treeView.layer)
//	
//		if ((treeView.clickCB)&&(!noSendClickCB)) treeView.clickCB(userData,isFromKeybArrow!=null?isFromKeybArrow:false)
//	}	
//}
// ================================================================================
function TreeWidgetElem_multiSelectCtrl()
{	
}
// ================================================================================
//select items between id1 and id2
//function TreeWidgetElem_multiSelectShift(id1,id2)
//{
//	var elem1=_TreeWidgetElemInstances[id1];
//	var elem2=_TreeWidgetElemInstances[id2];
//	var treeView = elem1?elem1.treeView:null;
//	
//	if(treeView == null) return;//safe test
//	
//	if (elem1.elemPos==-1 || elem2.elemPos==-1 ) treeView.buildElems()
//	var startPos= (elem1.elemPos<elem2.elemPos)?elem1.elemPos:elem2.elemPos
//	var endPos= (elem1.elemPos>elem2.elemPos)?elem1.elemPos:elem2.elemPos
//	
//	if ((startPos>=0)&&(endPos<treeView.elems.length))
//	{
//		for(var j=startPos;j<=endPos;j++)
//		{
//			var elem = treeView.elems[j];
//			
//			// don't select if there's a grayed item
//			//if (elem.isGrayStyle()) return		
//			
//			var hidden=elem.getHiddenParent();//not visible item
//							
//			if((hidden == null)&&(arrayFind(treeView,'selIds',elem.id) == -1))//safe test
//			{
//				//add to selected items array
//				treeView.selIds[treeView.selIds.length]=elem.id;					
//				if(treeView.layer._BOselIds == "")
//					treeView.layer._BOselIds=""+elem.id;
//				else
//					treeView.layer._BOselIds+=";"+elem.id;
//													
//				elem.init();			
//				if(elem.domElem)
//					elem.domElem.className=elem.selectedClass;	
//			}
//		}											
//	}
//}
// ================================================================================

var _treeWClickedW;
function TreeWidget_clickCB(elemId,isIcon,ev,isDown,expand)
{	
	eventCancelBubble(ev)
		
	var e=_TreeWidgetElemInstances[elemId]

	if (e==null)
		return
	
	e.init()
	var tree=e.treeView;
	
	tree.setFocus(elemId);

	if (expand != null) {
		if (e.expanded==expand) {
			// if the node is already expanded, focus on the first sub node
			if (expand && e.sub && e.sub.length > 0) {
				tree.setFocus(e.sub[0].id);
			}
			// if the node is already collapsed, focus on the parent node
			else if (!expand && e.par) {
				tree.setFocus(e.par.id);
			}
			return;
		}
	}
	
	if (isIcon) {
		if (e.sub.length > 0)
			TreeWidget_toggleCB(elemId, true);
		else if (e.isIncomplete && e.querycompleteCB)
			e.querycompleteCB();
		else
			return;
	}
	else {
		e.select(null,ev);
	}

	if (_curDoc.onmousedown)
		_curDoc.onmousedown(ev)

	if (isDown&&_ie)
	{
		_treeWClickedW=e
		e.init()
		e.clicked=true
		e.initialX=eventGetX(ev)
		e.initialY=eventGetY(ev)

		if (_ie&&e.domElem)
			e.domElem.onmousemove=TreeWidgetElem_triggerDD
	}

	return false
}

// ================================================================================

function treeDblClickCB(elemId,ev)
{
	eventCancelBubble(ev)
	var e=_TreeWidgetElemInstances[elemId],treeView=e.treeView;

	if (e.sub.length>0) TreeWidget_toggleCB(elemId)
	else if (e.isIncomplete&&e.querycompleteCB)
	{
		e.querycompleteCB()
		return
	}


	if (e.isEditable)
		e.showEditInput(true)
	else
	{
		if (treeView.doubleClickCB) treeView.doubleClickCB(e.userData);
	}

}

// ================================================================================

function TreeWidget_keydownCB(elemId,ev,k)
{
	eventCancelBubble(ev);
    
	var e=_TreeWidgetElemInstances[elemId],r,delta;
	var tree=e.treeView;

	switch (k)
	{
	case KEY_PAGEUP:
		if (tree.layer && e.layer) {
			delta = Math.max(0, Math.round(tree.layer.offsetHeight / e.layer.offsetHeight) - 1);
			r = e.getNextPrev(-delta);
			if (r) tree.setFocus(r.id);
		}
		break;
	case KEY_PAGEDOWN:
		if (tree.layer && e.layer) {
			delta = Math.max(0, Math.round(tree.layer.offsetHeight / e.layer.offsetHeight) - 1);
			r = e.getNextPrev(delta);
			if (r) tree.setFocus(r.id);
		}
		break;
	/*
	case KEY_END:
		r = tree.getLast();
		if (r) tree.setFocus(r.id);
		break;
	case KEY_HOME:
		r = tree.getFirst();
		if (r) tree.setFocus(r.id);
		break;
	*/
	case KEY_UP:
		r = e.getNextPrev(-1);
		if (r) tree.setFocus(r.id);
		break;
	case KEY_DOWN:
		r = e.getNextPrev(1);
		if (r) tree.setFocus(r.id);
		break;
	default:
		break;
	}
}

// ================================================================================

function TreeWidgetElem_UpdateTooltip(newId,forceSelect)
{
	var elem=_TreeWidgetElemInstances[newId];
	if(elem)
	{
		elem.init();
		if(elem.domElem != null)
			elem.domElem.title = elem.getTooltip(forceSelect);
	}
}

// ================================================================================

function TreeWidgetElem_getDragTooltip()
{
	var o=this
	if (o.obj && o.obj.getDragTooltip) return o.obj.getDragTooltip()
	return o.name
}

// ================================================================================

function TreeWidgetElem_getTooltip(forceSelect)
{
	var tooltip='',o=this
	
	var itemSelected=false;
	//multi selection
	if(o.treeView.multiSelection)	
	{
		itemSelected = (arrayFind(o.treeView,'selIds',o.id) > -1);
	}
	else//mono selection
	{
		itemSelected = (o.treeView.selId == o.id);
	}
	
	//selection
	if (forceSelect || itemSelected)
		tooltip = L_DHTMLLIB_selectedLab + ' ';		
/*
	if(o.tooltip)
		tooltip+=o.tooltip+' ';
*/
	//name
	//tooltip+=o.name;

	//expanded or collapsed
	if((o.sub.length > 0) || (o.isIncomplete))
	{		
		if(o.expanded)	
			tooltip += ' '+ L_DHTMLLIB_expandedLab + ' ';
		else				
			tooltip += ' '+ L_DHTMLLIB_collapsedLab + ' ';		
	}
	
	//case prompt and context selection
	if(o.advTooltip)
	{
		tooltip+=' ('+o.advTooltip+')'		
	}	
	
	// level info
	if(o.getLevel)
		tooltip += ' ('+L_DHTMLLIB_level+' '+o.getLevel()+')';
/*
	if(o.help)		
		tooltip +=' - ' + o.help;
*/
	return tooltip;
}

//2 possible workflows:
//1- display tooltip
//2- item has callback associated to mouse over
function _tmoc(e,elemId,over,ev)
{
	e.style.cursor=_hand
	var elem =_TreeWidgetElemInstances[elemId];
	if(elem==null)
		return


		
	//1: normal tooltip - mouse over callback	
	if(elem.treeView.mouseOverTooltip)
	{
		if(over)
		{
			if (elem.customTooltip && elem.showCustomTooltipCB)
			{
				elem.showCustomTooltipCB(elem.userData,ev)
				elem.init()			
				
			}
			else
				e.title = elem.tooltip?elem.tooltip:''
		}
		else
		{
			if (elem.customTooltip && elem.hideCustomTooltipCB)
				elem.hideCustomTooltipCB()
			else
				e.title =''
		}
	}
	//we can also do the mouseover action, it does not interfere with the tooltip action
	//asked by Bruno Rassamy for Mediance
	if(elem.treeView.mouseOverCB)
		elem.treeView.mouseOverCB(elem)
}


// 
function treeContextMenuCB(elemId,ev)
{	

	var elem =_TreeWidgetElemInstances[elemId];
	if(elem)
	{	
		elem.treeView.rightClickMenuCB(elemId, _ie?_curWin.event:ev)
	}
}

//return true if is leaf
function TreeWidgetElem_isLeaf()
{
	return (this.sub.length==0 && !this.isIncomplete);
}
//return true if is node
function TreeWidgetElem_isNode()
{
	return (!this.isLeaf());
}
//return true if selected item
function TreeWidgetElem_isSelected()
{
	var o=this;
	if(o.treeView.multi)//multi selection
	{
		var idx = arrayFind(o.treeView,'selIds',o.id);
		return (idx>=0);
	}
	else
	{
		return (o.id == o.treeView.selId);
	}
}

function TreeWidgetElem_setCursorClass(newCursorClass)
{
	this.cursorClass=newCursorClass;
}

// =====================
// TreeWidgetElem class
// =====================
/*
function newTreeWidgetHTMLElem(iconID, name, obj, userData, help)
{
	var o=newTreeWidgetElem(iconID, name, userData, help);
	o.obj=obj
	o.getHTML=TreeWidgetHTMLElem_getHTML;
	o.selectedClass='filterBoxSelected';
	o.nonselectedClass='filterBox';
	o.feedbackDDClass='filterBoxFeedbackDD'
	o.init=TreeWidgetHTMLElem_init
	
	return o;
}

function TreeWidgetHTMLElem_init()
{
	this.domElem=getLayer(_codeWinName+'trLstElt' + this.id)
}


function TreeWidgetHTMLElem_getHTML(indent,isFirst)
{

	with (this)
	{
		len=sub.length,exp=(len>0),a=new Array,i=0
		
		var contextMenu=''
		if (treeView.rightClickMenuCB != null)
		{
			contextMenu= ' oncontextmenu="' + _codeWinName + '.treeContextMenuCB(\''+ id + '\', event);return false" '
		}
		
		var acceptDD=''			
		if ((treeView.acceptDropCB != null) && (_ie))
		{						
			acceptDD= ' ondragenter="' + _codeWinName + '.TreeWidget_dragOverEnterCB(this,\''+id+'\');" '
			acceptDD += ' ondragover="' + _codeWinName + '.TreeWidget_dragOverEnterCB(this,\''+id+'\');" '
		}	
				
		a[i++] = '<table border="0" cellspacing="0" cellpadding="0"><tbody><tr>'
		a[i++] = '<td>' + getSpace(indent + 16, 16) + '</td>'
		a[i++] = '<td>'

		a[i++] =	'<table id="'+_codeWinName+'trLstElt' + id + '" class="filterBox" onclick="'+_codeWinName+'.TreeWidget_clickCB(\''+ id +'\',false,event,true);if(_saf||_ie) return false"  ondblclick="'+_codeWinName+'.treeDblClickCB('+id+',event);if(_saf||_ie) return false" ' + contextMenu + acceptDD + ' border="0" cellspacing="0" cellpadding="0">'
		a[i++] =		'<tbody><tr>'

		a[i++] =		'<td width="20">'
		a[i++] =			'<span style="padding:2px">'
		a[i++] =				(iconId>-1?imgOffset(treeView.icns,treeView.iconW,treeView.iconH, treeView.iconW * iconId, treeView.iconH * iconId,null,null,null,'','top'):'')
		a[i++] =			'</span>'
		a[i++] =		'</td>'

		//					see	BOFilterNode_getHTML in CommonWom.js
		//					BOFilterNode_getHTML(treeView, filterID)
		a[i++] =		'<td>' + obj.getHTML(treeView, userData) + '</td>'

		a[i++] =	'</tr></tbody></table>'

		a[i++] = '</td></tr></tbody></table>'

	}


	return  a.join("");
}


//

function newTreeWidgetVoidElem(iconID, name, obj, userData, help)
{
	var o=newTreeWidgetElem(iconID, name, userData, help);
	o.obj=obj
	o.getHTML=TreeWidgetVoidElem_getHTML;
	o.selectedClass='filterTextSelected';
	o.nonselectedClass='filterText';
	o.feedbackDDClass='filterTextFeedbackDD '
	o.init=TreeWidgetVoidElem_init

	return o;
}

function TreeWidgetVoidElem_init()
{
	this.domElem=getLayer(_codeWinName+'trLstElt' + this.id)
}

function TreeWidgetVoidElem_getHTML(indent,isFirst)
{
	return  ""
}

// =================
// TreeWidget class
// =================

function newStaticTreeWidget(id, originalTree)
{
	var o = newWidget(id);	
	o.sub = originalTree.sub;
	return o;
}



// =================
// FolderWidget class
// =================
function newFolderWidget(id, exp)
// Return [FolderWidget] the instance
{
	var o=newWidget(id)
	o.expanded= (exp)? exp : false
	o.getCross=FolderWidget_getCross	
	return o;
}

//
// function getCross(id)
//
//
function FolderWidget_getCross()
{
	if (_printDoc) return getSep(12,12)
				
	with(this)
	{
		return '<span style="cursor:'+_hand+'" id="icn'+id+'" '+(_moz?'onclick':'onmousedown')+'="'+_codeWinName+'.clickCrossCB('+id+'); if (_ie) return false" ondblclick="'+_codeWinName+'.clickCrossCB('+id+'); if (_ie) return false">' +
					imgOffset(_skin +'../tree.gif', 13, 9, expanded?0:13, 3, 'img'+ id, null, expanded?L_DHTMLLIB_expandedLab:L_DHTMLLIB_collapsedLab,null,null,'top') +
				'</span>'
	}
											
}



*/

DHTMLLIB.add(_tmvc,_tkt,_tpt, _tpl,TreeWidget_dragOverEnterCB,TreeWidgetElem_iconFocusCB,TreeWidget_focusCB);
