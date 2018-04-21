node {
	stage 'Checkout'
		checkout scm

	stage 'Build'
		bat 'nuget restore EcoWorldEdit.sln'
		bat "\"${tool 'MSBuild'}\" EcoWorldEdit.sln /p:Configuration=Release /p:Platform=\"Any CPU\" /p:ProductVersion=1.0.0.${env.BUILD_NUMBER}"

	stage 'Archive'
		archive 'EcoWorldEdit/bin/Release/**'

}