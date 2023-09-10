import { Injectable } from "@angular/core";
import { EMPTY, Observable, Subject, Subscription, defer, filter, map, mergeWith, take, takeWhile, tap, using } from "rxjs";
import { makeid } from "../utilities/makeid";
import { RequestType } from "../generated-client/request-type";
import { ResponseDto } from "../generated-client/response-dto";
import { RequestDto } from "../generated-client/request-dto";
import { ResponseType } from "../generated-client/response-type";

@Injectable()
export class MessageService {
 
  private readonly receivedMessageSub = new Subject<ResponseDto>();

  constructor() {
    (window as any).chrome.webview.addEventListener('message', (arg: any) => {
      this.receivedMessageSub.next(arg.data);
    })
  }

  receivedMessage(requestId: string): Observable<ResponseDto> {
    return this.receivedMessageSub.pipe(
      filter(data => data.requestId === requestId),
    );
  }

  callNativeFunction<TRes, TReq extends any[]>(controllerName: string, functionName: string, data: TReq): Observable<TRes> {
    const requestData: RequestDto = {
      controller: controllerName,
      function: functionName,
      requestId: makeid(22),
      requestType: RequestType.FunctionCall,
      data: data.map(d => JSON.stringify(d))
    };

    return this.receivedMessage(requestData.requestId).pipe(
      mergeWith(defer(() => {
        (window as any).chrome.webview.postMessage(requestData);
        return EMPTY;
      })),
      map(data => {
        if (data.responseType == ResponseType.Error) {
          throw new Error(data.data);
        }
        return JSON.parse(data.data ?? 'null');
      }),
      take(1)
    );
  }

  callNativeSubscribe<TRes, TReq extends any[]>(controllerName: string, functionName: string, data: TReq): Observable<TRes> {
    const requestData: RequestDto = {
      controller: controllerName,
      function: functionName,
      requestId: makeid(22),
      requestType: RequestType.Subscription,
      data: data.map(d => JSON.stringify(d))
    };

    return new Observable(subscriber => {
      let completed = false;
      var subscription = new Subscription(() => {
        if (!completed) {
          (window as any).chrome.webview.postMessage({
            ...requestData,
            requestType: RequestType.Unsubscription,
            data: []
          })
        }
      });
      subscription.add(this.receivedMessage(requestData.requestId).pipe(
        mergeWith(defer(() => {
          (window as any).chrome.webview.postMessage(requestData);
          return EMPTY;
        })),
        tap(data => {
          if (data.responseType != ResponseType.Success) {
            completed = true;
          }
        }),
        takeWhile(data => data.responseType != ResponseType.Completed),
        map(data => {
          if (data.responseType == ResponseType.Error) {
            throw new Error(data.data);
          }
          return JSON.parse(data.data ?? 'null');
        })
      ).subscribe(subscriber));
      return subscription;
    });
  }
}
