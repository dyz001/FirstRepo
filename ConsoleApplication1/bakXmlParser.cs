﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//class XMLParser{
	
//    private char LT ='<'[0];
//    private char GT='>'[0];
//    private char SPACE=' '[0];
//    private char QUOTE='\''[0];
//    private char SLASH='/'[0];
//    private char QMARK='?'[0];
//    private char EQUALS='='[0];
//    private char EXCLAMATION='!'[0];
//    private char DASH='-'[0];
//    //private char SQL='['[0];
//    private char SQR=']'[0];
	
//    public void Parse(string content){

//        var rootNode:XMLNode=new XMLNode();
//        rootNode["_text"]="";

//        var nodeContents:String="";
		
//        var inElement:boolean=false;
//        var collectNodeName:boolean=false;
//        var collectAttributeName:boolean=false;
//        var collectAttributeValue:boolean=false;
//        var quoted:boolean=false;
//        var attName:String="";
//        var attValue:String="";
//        var nodeName:String="";
//        var textValue:String="";
		
//        var inMetaTag:boolean=false;
//        var inComment:boolean=false;
//        var inDoctype:boolean=false;
//        var inCDATA:boolean=false;
		
//        var parents:XMLNodeList=new XMLNodeList();
		
//        var currentNode:XMLNode=rootNode;
//        for(var i=0;i<content.length;i++){
			
//            var c:char=content[i];
//            var cn:char;
//            var cnn:char;
//            var cp:char;
//            if((i+1)<content.length) cn=content[i+1]; 
//            if((i+2)<content.length) cnn=content[i+2]; 
//            if(i>0)cp=content[i-1];
					
//            if(inMetaTag){
//                if(c==QMARK && cn==GT){
//                    inMetaTag=false;
//                    i++;
//                }
//                continue;
//            }else{
//                if(!quoted && c==LT && cn==QMARK){
//                    inMetaTag=true;
//                    continue;	
//                }	
//            }
			
//            if(inDoctype){				
//                if(cn==GT){
//                    inDoctype=false;
//                    i++;
//                }
//                continue;
//            }else if(inComment){
//                if(cp==DASH && c==DASH && cn==GT){
//                    inComment=false;
//                    i++;
//                }
//                continue;	
//            }else{
//                if(!quoted && c==LT && cn==EXCLAMATION){					
//                    if(content.length>i+9 && content.Substring(i,9)=="<![CDATA["){
//                        inCDATA=true;
//                        i+=8;
//                    }else if(content.length > i+9 && content.Substring(i,9)=="<!DOCTYPE"){
//                        inDoctype=true;						
//                        i+=8;					
//                    }else if(content.length > i+2 && content.Substring(i,4)=="<!--"){					
//                        inComment=true;
//                        i+=3;
//                    }
//                    continue;	
//                }
//            }
			
//            if(inCDATA){
//                if(c==SQR && cn==SQR && cnn==GT){
//                    inCDATA=false;
//                    i+=2;
//                    continue;
//                }
//                textValue+=c;
//                continue;	
//            }
			
			
//            if(inElement){
//                if(collectNodeName){
//                    if(c==SPACE){
//                        collectNodeName=false;
//                    }else if(c==GT){
//                        collectNodeName=false;
//                        inElement=false;
//                    }
					
			
		
//                    if(!collectNodeName && nodeName.length>0){
//                        if(nodeName[0]==SLASH){
//                            // close tag
//                            if(textValue.length>0){
//                                currentNode["_text"]+=textValue;
//                            }
					
//                            textValue="";
//                            nodeName="";
//                            currentNode=parents.Pop();
//                        }else{

//                            if(textValue.length>0){
//                                currentNode["_text"]+=textValue;
//                            }
//                            textValue="";	
//                            var newNode:XMLNode=new XMLNode();
//                            newNode["_text"]="";
//                            newNode["_name"]=nodeName;
							
//                            if(!currentNode[nodeName]){
//                                currentNode[nodeName]=new XMLNodeList();	
//                            }
//                            var a:Array=currentNode[nodeName];
//                            a.Push(newNode);	
//                            parents.Push(currentNode);
//                            currentNode=newNode;
//                            nodeName="";
//                        }
//                    }else{
//                        nodeName+=c;	
//                    }
//                }else{
					
//                    if(!quoted && c==SLASH && cn==GT){
//                        inElement=false;
//                        collectAttributeName=false;
//                        collectAttributeValue=false;	
//                        if(attName){
//                            if(attValue){
//                                currentNode["@"+attName]=attValue;								
//                            }else{
//                                currentNode["@"+attName]=true;								
//                            }
//                        }
						
//                        i++;
//                        currentNode=parents.Pop();
//                        attName="";
//                        attValue="";		
//                    }
//                    else if(!quoted && c==GT){
//                        inElement=false;
//                        collectAttributeName=false;
//                        collectAttributeValue=false;	
//                        if(attName){
//                            currentNode["@"+attName]=attValue;							
//                        }
						
//                        attName="";
//                        attValue="";	
//                    }else{
//                        if(collectAttributeName){
//                            if(c==SPACE || c==EQUALS){
//                                collectAttributeName=false;	
//                                collectAttributeValue=true;
//                            }else{
//                                attName+=c;
//                            }
//                        }else if(collectAttributeValue){
//                            if(c==QUOTE){
//                                if(quoted){
//                                    collectAttributeValue=false;
//                                    currentNode["@"+attName]=attValue;								
//                                    attValue="";
//                                    attName="";
//                                    quoted=false;
//                                }else{
//                                    quoted=true;	
//                                }
//                            }else{
//                                if(quoted){
//                                    attValue+=c;	
//                                }else{
//                                    if(c==SPACE){
//                                        collectAttributeValue=false;	
//                                        currentNode["@"+attName]=attValue;								
//                                        attValue="";
//                                        attName="";
//                                    }	
//                                }
//                            }
//                        }else if(c==SPACE){
						
//                        }else{
//                            collectAttributeName=true;							
//                            attName=""+c;
//                            attValue="";
//                            quoted=false;		
//                        }	
//                    }
//                }
//            }else{
//                if(c==LT){
//                    inElement=true;
//                    collectNodeName=true;	
//                }else{
//                    textValue+=c;	
//                }	
				
//            }
				
//        }
	
//        return rootNode;
//    }

//}

//class XMLNode extends Boo.Lang.Hash{
//    function GetNodeList(path:String){
//        return GetObject(path) as XMLNodeList;
//    }
	
//    function GetNode(path:String){
//        return GetObject(path) as XMLNode;
//    }
	
//    function GetValue(path:String){
//        return GetObject(path) as String;
//    }
	
//    private function GetObject(path:String){
//        var bits:String[]=path.Split(">"[0]);
//        var currentNode:XMLNode=this;
//        var currentNodeList:XMLNodeList;
//        var listMode:boolean=false;
//        var ob:Object;
//        for(var i:int=0;i<bits.length;i++){
//             if(listMode){
//                ob=currentNode=currentNodeList[parseInt(bits[i])];
//                listMode=false;
//             }else{
//                ob=currentNode[bits[i]];
//                if(ob instanceof Array){
//                    currentNodeList=ob as Array;
//                    listMode=true;
//                }else{
//                    // reached a leaf node/attribute
//                    if(i!=(bits.length-1)){
//                        // unexpected leaf node
//                        var actualPath:String="";
//                        for(var j:int;j<=i;j++){
//                            actualPath=actualPath+">"+bits[j];
//                        }
//                        Debug.Log("xml path search truncated. Wanted: "+path+" got: "+actualPath);
//                    }
//                    return ob;
//                }
//             }
//        }
//        if(listMode) return currentNodeList;
//        else return currentNode;
//    }
//}