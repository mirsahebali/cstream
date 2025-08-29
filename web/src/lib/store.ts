import * as signalR from '@microsoft/signalr';
import { to } from './utils';


export function getSignalRConnection() {

  const signalRConnection = new signalR.HubConnectionBuilder()
    .withUrl(to('/stream'), { withCredentials: true })
    .build();
  return signalRConnection
}
