// Copyright (c) 2010 Martin Knafve / hMailServer.com.  
// http://www.hmailserver.com

#pragma once

#include "../common/Util/File.h"
#include "../common/Util/TransparentTransmissionBuffer.h"
#include "../Common/TCPIP/AnsiStringConnection.h"

namespace HM
{
   class Messages;
   class ByteBuffer;

   class POP3Connection : public AnsiStringConnection
   {
   public:

      POP3Connection(ConnectionSecurity connection_security,
         boost::asio::io_service& io_service, 
         boost::asio::ssl::context& context);
	   virtual ~POP3Connection();

      virtual void ParseData(const AnsiString &Request);
      virtual void ParseData(shared_ptr<ByteBuffer> pBuffer) { };

      virtual void OnDataSent();

   protected:

      virtual void OnConnected();
      virtual AnsiString GetCommandSeparator() const;

      virtual void _SendData(const String &sData) ;
      virtual void _SendDataDebugOnly(const String &sData) ;

      virtual void OnDisconnect();
      virtual void OnConnectionTimeout();
      virtual void OnExcessiveDataReceived();
      virtual void OnHandshakeCompleted();
      virtual void OnHandshakeFailed() {};
   private:


      enum ParseResult
      {
         ResultNormalResponse = 0,
         ResultStartSendMessage = 1,
         ResultDisconnect = 2,
         ResultStartTls = 3
      };

      enum POP3Command
      {
         INVALID = 0,
         NOOP = 1,
         USER = 2,
         PASS = 3,
         HELP = 4,
         QUIT = 5,
         STAT = 6,
         LIST = 7,
         RETR = 8,
         TOP = 9,
         RSET = 10,
         DELE = 11,
         UIDL = 12,
         CAPA = 13,
         STLS = 14
      };

      enum ConnectionState
      {
         AUTHORIZATION = 1,
         TRANSACTION = 2,
         UPDATE = 3,
      };

      ParseResult InternalParseData(const AnsiString &Request);

      POP3Command GetCommand(ConnectionState currentState, String command);

      void _LogClientCommand(const String &sClientData);
      void _GetMailboxContents(int &iNoOfMessages, __int64 &iTotalBytes);

      void SendBanner_();
      ParseResult _ProtocolRETR(const String &Parameter);
      bool _ProtocolLIST(const String &sParameter);
      bool _ProtocolDELE(const String &Parameter);
      void _ProtocolUSER(const String &Parameter);
      ParseResult _ProtocolPASS(const String &Parameter);
      bool _ProtocolTOP(const String &Parameter);
      bool _ProtocolUIDL(const String &Parameter);
      bool _ProtocolSTAT(const String &sParameter);
      void _ProtocolRSET();
      void _ProtocolQUIT();
      bool _ProtocolSTLS();
      void _ProtocolCAPA();

      bool _SendFileHeader(const String &sFilename, int iNoOfLines = 0);
      bool _ReadLine(HANDLE hFile, const String &sLine);

      void _SaveMailboxChanges();
      void _UnlockMailbox();

      void _StartSendFile(shared_ptr<Message> message);
	  void _ReadAndSend();
      void _ResetMailbox();
      shared_ptr<Message> _GetMessage(unsigned int index);

      String m_Username;
      String m_Password;

      shared_ptr<const Account> _account;

      ConnectionState m_CurrentState;

      std::vector<shared_ptr<Message>> _messages;
      TransparentTransmissionBuffer m_oTransmissionBuffer;

      bool m_bPendingDisconnect;
      File _currentFile;
   };
}
