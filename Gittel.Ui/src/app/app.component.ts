import { Component, Inject } from '@angular/core';
import { ISampleClient, SAMPLE_CLIENT_TOKEN } from '../generated-client/sample-client';
import { take } from 'rxjs';
import { REPOSITORY_CLIENT_TOKEN, IRepositoryClient } from '../generated-client/repository-client';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  public repo: string | undefined;
  public repoWorkDir: string | undefined;

  constructor(
    @Inject(SAMPLE_CLIENT_TOKEN) private readonly sampleClient: ISampleClient,
    @Inject(REPOSITORY_CLIENT_TOKEN) private readonly repositoryClient: IRepositoryClient,
  ) {
  }

  public discoverRepo() {
    this.repositoryClient.discoverRepository({ basePath: "G:\\Projects\\test-repo-worktree\\asdf\\a" })
      .subscribe(res => {
        console.log(res);
        this.repo = res.path;
        this.repoWorkDir = res.workDir;
      })
  }

  public foo() {

    this.sampleClient.sampleFunction({
      text: "TestString"
    }).subscribe(res => {
      console.log(res);
    })

  }

  public foo2() {

    this.sampleClient.returnVoid({
      text: "TestString"
    }, {
      number: 5
    }).subscribe(res => {
      console.log(res);
    })

  }

  public foo3() {

    this.sampleClient.sampleEvent({
      text: "TestString"
    }).pipe(take(5)
    ).subscribe(
      res => { console.log(res); },
      err => { console.log(err); },
      () => console.log('completed')
    );
  }

  public foo4() {

    this.sampleClient.sampleEventWithMultipleParameters({
      number: 5
    },
    {
      text: "TestString"
    }).pipe(take(5)
    ).subscribe(
      res => { console.log(res); },
      err => { console.log(err); },
      () => console.log('completed')
    );
    
  }
}
