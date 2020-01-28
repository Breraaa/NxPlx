import orderBy from "lodash/orderBy";
import { Component, h } from "preact";
import Helmet from "preact-helmet";
import Entry from "../../components/Entry";
import Loading from "../../components/Loading";
import http from "../../utils/http";
import { imageUrl, MovieCollection } from "../../utils/models";
import * as style from "./style.css";

interface Props {
    id: string
}

interface State {
    details: MovieCollection,
    bg: string
}

export default class Collection extends Component<Props, State> {
    public componentDidMount(): void {
        http.get(`/api/film/collection/detail/${this.props.id}`)
            .then(response => response.json())
            .then((details: MovieCollection) => {
                console.log(details);
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(details.backdrop, 1280)}");`;
                this.setState({ details, bg });
            });
    }


    public render(_, { details, bg }: State) {
        if (!details) {
            return (<Loading fullscreen/>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={details.backdrop}>
                <div class={`nx-scroll ${style.content}`}>
                    <Helmet title={`${details.name} - NxPlx`}/>
                    <div>
                        <h2 class={[style.title, style.marked].join(" ")}>{details.name}</h2>
                    </div>
                    <img class={style.poster} src={imageUrl(details.poster, 500)} alt=""/>
                    <div>
                        {orderBy(details.movies, ["year", "title"], ["asc"])
                            .map(movie => (
                                    <Entry
                                        key={movie.id}
                                        title={movie.title}
                                        href={`/app/${movie.kind}/${movie.id}`}
                                        image={imageUrl(movie.poster, 342, details.poster)}/>

                                )
                            )}
                    </div>
                </div>
            </div>
        );
    }
}
