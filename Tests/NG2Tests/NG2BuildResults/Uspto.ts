import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
export namespace MyNS {
	export interface DataSetList {
		total?: number;
		DataSetListApis?: Array<DataSetListApis>;
	}

	export interface DataSetListApis {

		/** To be used as a dataset parameter value */
		apiKey?: string;

		/** To be used as a version parameter value */
		apiVersionNumber?: string;

		/** The URL describing the dataset's fields */
		apiUrl?: string;

		/** A URL to the API console for each API */
		apiDocumentationUrl?: string;
	}

	@Injectable()
	export class MyClient {
		constructor(@Inject('baseUri') private baseUri: string = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + '/', private http: HttpClient) {
		}

		/**
		 * List available data sets
		 * Get 
		 * @return {DataSetList} Returns a list of data sets
		 */
		List_data_sets(): Observable<DataSetList> {
			return this.http.get<DataSetList>(this.baseUri + '', {});
		}

		/**
		 * Provides the general information about the API and the list of fields that can be used to query the dataset.
		 * This GET API returns the list of all the searchable field names that are in the oa_citations. Please see the 'fields' attribute which returns an array of field names. Each field or a combination of fields can be searched using the syntax options shown below.
		 * Get {dataset}/{version}/fields
		 * @param {string} dataset Name of the dataset.
		 * @param {string} version Version of the dataset.
		 * @return {string} The dataset API for the given version is found and it is accessible to consume.
		 */
		List_searchable_fields(dataset: string, version: string): Observable<string> {
			return this.http.get(this.baseUri + (dataset == null ? '' : encodeURIComponent(dataset)) + '/' + (version == null ? '' : encodeURIComponent(version)) + '/fields', { responseType: 'text' });
		}
	}

}

